using [custom];
using [custom].Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace [custom]
{
    public class UserDetail
    {
        public class Query : IRequest<Model>
        {
            public int ID { get; set; }
        }

        public class Model
        {
            public int ID { get; set; }
            public string UserID { get; set; }

            [DisplayName("First Name")]
            public string FirstName { get; set; }

            [DisplayName("Last Name")]
            public string LastName { get; set; }

            [DisplayName("Role")]
            public string Role { get; set; }
            public bool IsActive { get; set; }

            [DisplayName("Title")]
            public string Title { get; set; }

            [DisplayName("Email Address")]
            public string EmailAddress { get; set; }
            [DisplayName("User Color")]
            public string Color { get; set; }
            [DisplayName("Calendar Name")]
            public string CalendarName { get; set; }
            [DisplayName("Phone Number")]
            public string PhoneNumber { get; set; }

            [DisplayName("Is Active")]
            public string DisplayIsActive
            {
                get { return this.IsActive ? "Yes" : "No"; }
            }
            public string ColorCSS
            {
                get
                {
                    return $"style=color:{this.Color};font-weight:bold";
                }
            }
            public IList<AppUserPermission> UserPermissions { get; set; }
        }

        public class AppUser
        {
            public int ID { get; set; }
            public string UserID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Role { get; set; }
            public bool IsActive { get; set; }
            public string Title { get; set; }
            public string EmailAddress { get; set; }
            public string Color { get; set; }
            public string CalendarName { get; set; }
            public string PhoneNumber { get; set; }
            public DateTime UpdateDT { get; set; }
        }

        public class AppUserPermission
        {
            public int ID { get; set; }
            public int AppUserID { get; set; }
            public int AppPermissionID { get; set; }
            public string Name { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, Model>
        {
            private readonly IAppDbContext _context;
            private readonly IAppUser _user;
            public QueryHandler(IAppDbContext context, IAppUser user)
            {
                _context = context;
                _user = user;
            }

            public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
            {
                var sql = @"
SELECT ID
        ,UserID
        ,FirstName
        ,LastName
        ,[Role]
        ,IsActive
        ,Title
        ,EmailAddress
        ,Color
        ,CalendarName
        ,PhoneNumber
FROM [DummyUserTable] WHERE ID = @ID

SELECT up.ID
        ,up.AppUserID
        ,up.AppPermissionID
	    ,p.[Name]
FROM [DummyPermissionTable] up
INNER JOIN sec.AppPermission p on up.AppPermissionID = p.ID
WHERE AppUserID = @ID
ORDER BY p.[Name]";


                var multi = await _context.QueryMultiple(sql, new { ID = request.ID }); // Dapper has Query multiple calls for multiple queries
                var model = multi.Read<Model>().FirstOrDefault();
                var userPermissions = multi.Read<AppUserPermission>().ToList();
                model.UserPermissions = userPermissions;
     
                return model;
            }

            private async Task<AppUser> GetUser(int ID)
            {
                var sql = @"
SELECT ID
    ,UserID
    ,FirstName
    ,LastName
    ,[Role]
    ,IsActive
    ,Title
    ,EmailAddress
FROM [DummyUserTable] WHERE ID = @ID";

                var query = await _context.Query<AppUser>(
                   sql,
                   new
                   {
                       ID = ID
                   }
               );

                return query.FirstOrDefault();
            }
        }
    }
}
