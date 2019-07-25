using [custom].AspNetCore;
using [custom].AspNetCore.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace [custom]
{
    /*
    Hi John, this is a sample class designed using the Command Query Seperation (CQS) pattern.
    The architecture of most applications use a Domain Driven Design approach which aggregates dependent 
    classes or modules of similar functionality together. (you'll notice mutiple classes within the file)
    I use nuget packages such as MediatR to route calls between the Controller and this domain class and 
    FluentValidation to validate input.
    Besides the 2 mentioned, I also have custom nuget package [custom].AspNetCore nuget pkg that I 
    inject the same way accross multiple projects
    This custom package contains Data Access Layer, Security, Logging an Extensions shared amongst the applications
    this saves me time when setting up new projects.

    Due to NDA I've hidden a few parts of the code

    Please feel free to reach out to me if you find this useful or would like to discuss further.
    */
    public class AddUser
    {
        public class Query : IRequest<Command>
        { }

        public class Command : IRequest<CommandResult>
        {
            public Command()
            {
                Roles = new List<string>();
            }

            [DisplayName("User ID")]
            public string UserID { get; set; }

            [DisplayName("First Name")]
            public string FirstName { get; set; }

            [DisplayName("Last Name")]
            public string LastName { get; set; }

            [DisplayName("Role")]
            public string Role { get; set; }

            [DisplayName("Title")]
            public string Title { get; set; }

            [DisplayName("Email Address")]
            public string EmailAddress { get; set; }
            [DisplayName("Color")]
            public string Color { get; set; }
            [DisplayName("Calendar Name")]
            public string CalendarName { get; set; }
            [DisplayName("Phone Number")]
            public string PhoneNumber { get; set; }
            public IList<string> Roles { get; set; }
            public IList<SelectListItem> RolesSelectListItems
            {
                get
                {
                    var list =
                        from role in this.Roles
                        orderby role
                        select new SelectListItem
                        {
                            Text = $"{role}",
                            Value = role
                        };

                    return list.ToList();
                }
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator() // Fluent validation installed as a nuget package. Works great with Core and can be easiy unit tested
            {
                RuleFor(x => x.UserID)
                    .NotEmpty();

                RuleFor(x => x.FirstName)
                    .NotEmpty();

                RuleFor(x => x.LastName)
                    .NotEmpty();

                RuleFor(x => x.Role)
                    .NotEmpty();

                RuleFor(x => x.Title)
                    .NotEmpty();

                RuleFor(x => x.EmailAddress)
                    .NotEmpty()
                    .EmailAddress()
                    .WithMessage("A valid email address is required.");

            }
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
        //AppDbContext is a wrapper around the Dapper calls to the db. I use this across multiple projects to transact with the app database(s)
        private readonly IAppDbContext _context; 

            public QueryHandler(IAppDbContext context)// dependency injection
            {
                _context = context; // _context instantiated
            }
            public async Task<Command> Handle(Query request, CancellationToken cancellationToken)
            {
                var command = new Command
                {
                    Roles = await GetRoles()
                };

                return command;
            }

            private async Task<IList<string>> GetRoles()
            {
                var sql = @"SELECT DISTINCT [Role] FROM sec.AppUser order by [Role]";
                var query = await _context.Query<string>(sql); //Dapper Query command
                return query.ToList();
            }
        }

        public class CommandHandler : IRequestHandler<Command, CommandResult>
        {
            private readonly IAppDbContext _context;
            private readonly IAppUser _user;

            public CommandHandler(IAppDbContext context, IAppUser user) // dependency injection
            {
                _context = context;
                _user = user;
            }
             
            public async Task<CommandResult> Handle(Command command, CancellationToken cancellationToken)
            {
                bool userExists = await CheckIfUserExists(command.UserID);

                if (userExists)
                    return CommandResult.Fail("User already exists.");

                var ID = await InsertUser(command);


                return CommandResult.Success(ID);
            }

            private async Task<bool> CheckIfUserExists(string userID)
            {
                var sql = @"SELECT 1 FROM sec.AppUser WHERE UserID = @UserID";
                var query = await _context.Query<int?>(
                    sql, 
                    new
                    {
                        UserID = userID
                    }
                );
                return query.Any();
            }
            //Dapper allows ou to write out SQL Queries
            private async Task<int> InsertUser(Command command)
            {
                var sql = @"
DECLARE @InsertedRows TABLE ( ID int )
INSERT INTO sec.AppUser OUTPUT inserted.ID INTO @InsertedRows 
VALUES 
( 
    @UserID,
    @FirstName,
    @LastName,
    @Role,
    @IsActive,
    @Title,
    @EmailAddress,
   ...[some proeprties hidden]
)
SELECT ID FROM @InsertedRows";
                
                var result = await _context.Execute( //parameterized queries
                    sql,
                    new
                    {
                        UserID = command.UserID,
                        FirstName = command.FirstName,
                        LastName = command.LastName,
                        Role = command.Role,
                        IsActive = true,
                        Title = command.Title,
                        EmailAddress = command.EmailAddress,
                         //...[some properties hidden]
                        UpdatedUserID = _user.UserName,
                        UpdateDT = DateTime.Now
                    });

                return result;
            }

        }
    }
}
