using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using Sunday.Models;

namespace Sunday.App_Start
{

    //修改模型，重设数据库，并初始化数据
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            //InitializeIdentityForEF(context);
            base.Seed(context);

            //context.SysDepts.AddOrUpdate(d => d.DeptName,
            //    new SySDept() { DeptName = "总部" });
            var adminId = "bd4c3cc4-3fa8-45f5-b2c0-0f74cb1a3bf7";
            context.Users.AddOrUpdate(a => a.UserName,
                new ApplicationUser
                {
                    Id = adminId,
                    RegisterType = "0",
                    UserType = "0",
                    NickName = "超级管理员",
                    UserName = "admin",
                    PasswordHash = "AEN3E/gLflniKF22GOY2YwXm3Azc/l6Xt/BftIU23sqt4/EubckxHRw6UZbjvDV9FA==",
                    SecurityStamp = "e678a9e9-af5e-4c7b-8e8e-f63f664d7bc7",
                    CreateDate = DateTime.Now,
                    IsDeleted = false,
                    IsEnable = true
                });

            context.SysRoles.AddOrUpdate(a => a.RoleName,
                new SysRole { RoleName = "系统管理员", CreateUserId = adminId, CreateDate = DateTime.Now, IsDeleted = false, IsEnable = true },
                new SysRole { RoleName = "下线统计员", CreateUserId = adminId, CreateDate = DateTime.Now, IsDeleted = false, IsEnable = true },
                new SysRole { RoleName = "库管员", CreateUserId = adminId, CreateDate = DateTime.Now, IsDeleted = false, IsEnable = true },
                new SysRole { RoleName = "财务", CreateUserId = adminId, CreateDate = DateTime.Now, IsDeleted = false, IsEnable = true });
        }

        /// <summary>
        /// 数据初始化
        /// </summary>
        /// <param name="dbContext"></param>
        public static void InitializeIdentityForEF(ApplicationDbContext dbContext)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            const string name = "admin";//用户名
            //const string email = "admin@123.com";//邮箱
            const string password = "123456";//密码

            //如果没有admin用户则创建该用户
            try
            {
                var user = userManager.FindByName(name);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        Email = name,
                        UserName = name,
                        UserType = "0",
                        PasswordHash = password,
                        NickName = name
                    };
                    var result = userManager.Create(user, password);
                    result = userManager.SetLockoutEnabled(user.Id, false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            //创建角色列表
            var roles = new List<SysRole> {
                new SysRole { RoleName="系统管理员"},
                new SysRole { RoleName="下线统计员" },
                new SysRole { RoleName="库管员" },
                new SysRole { RoleName="财务" }
            };
            //遍历列表，如果数据库中不存在列表中某个角色，就添加角色
            foreach (var role in roles)
            {
                var _role = dbContext.SysRoles.Where(v => v.RoleName == role.RoleName).FirstOrDefault();
                if (_role == null)
                {
                    var result = dbContext.SysRoles.Add(role); // 创建角色
                }
            }
            dbContext.SaveChanges();
        }
    }
}