using  [custom].AspNetCore;
using  [custom].AspNetCore.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace [custom]
{
    public class GetUsers
    {
        public class Query : IRequest<Model>
        {
            public bool? ActiveUsersOnly { get; set; }
        }

        public class Model
        {
            public Model()
            {
                Users = new List<IndexUser>();
            }
            public IList<IndexUser> Users { get; set; }
            [Display(Name = "User Type")]
            public bool? ActiveUsersOnly { get; set; }

            public bool HasBLGUserAdminPermission { get; set; }

            public IList<SelectListItem> UserTypeListItems
            {
                get
                {
                    var list = new List<SelectListItem>
                    {
                        new SelectListItem { Text = "All Users", Value = "" },
                        new SelectListItem { Text = "Active Users", Value = true.ToString() }
                    };

                    return list;
                }
            }
        
        }

        public class IndexUser
        {
            public int ID { get; set; }
            public string UserID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Role { get; set; }
            public string Title { get; set; }
            public string Color { get; set; }
            public bool IsActive { get; set; }
           //some properties hidden
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
                var model = new Model
                {
                    Users = await GetUsers(request.ActiveUsersOnly),
                };

                return model;
            }
            public async Task<IList<IndexUser>>GetUsers(bool? IsActive)
            {
                var sql = @"
SELECT ID
    , UserID
    , FirstName
    , LastName
    , [Role]
    , IsActive
    , Title
    , Color
FROM [UserTable]
WHERE @IsActive IS NULL OR IsActive = @IsActive";

                var query = await _context.Query<IndexUser>(
                     sql,
                        new
                        {
                            IsActive = IsActive
                        }
                    );
                return query.OrderByDescending(b => b.UserID).ToList();
            }
        }
    }
}
