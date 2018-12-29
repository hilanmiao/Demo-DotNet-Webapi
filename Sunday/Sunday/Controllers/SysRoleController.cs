using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Sunday.App_Start;
using Sunday.Models;

namespace Sunday.Controllers
{
    /// <summary>
    /// 角色
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Role")]
    public class SysRoleController : ApiController
    {
        // NLog
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private ApplicationDbContext context = new ApplicationDbContext();

        /// <summary>
        /// 获取全部角色
        /// </summary>
        /// <returns></returns>
        [Route("GetAll")]
        public CommonClassPaged GetAll()
        {
            var list = context.SysRoles.Where(t => t.IsDeleted == false).OrderByDescending(t => t.CreateDate).ToList();
            var filteredList = from t in list
                               select new
                               {
                                   t.Id,
                                   t.RoleName,
                                   t.Menus,
                                   t.CreateDate
                               };
            // 处理返回值
            var CommonClassPaged = new CommonClassPaged
            {
                totalCount = list.Count,
                data = filteredList
            };
            return CommonClassPaged;
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页/条数</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        [Route("GetList")]
        public CommonClassPaged GetList(int pageIndex, int pageSize, string name = "")
        {
            // 处理逻辑
            var list = context.SysRoles.Where(t => t.IsDeleted == false).OrderBy(t => t.CreateDate).ToList();
            if (!string.IsNullOrEmpty(name))
            {
                list = list.Where(t => t.RoleName.Contains(name)).ToList();
            }

            var totalCount = list.Count();
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            var filteredList = from t in list
                               select new
                               {
                                   t.Id,
                                   t.RoleName,
                                   t.Menus
                               };

            // 处理返回值
            var CommonClassPaged = new CommonClassPaged
            {
                totalCount = totalCount,
                data = filteredList
            };
            return CommonClassPaged;
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Post")]
        public IHttpActionResult Post(TempSysRole model)
        {
            // 通过token获取当前用户
            var userId = User.Identity.GetUserId();

            // 处理逻辑
            var newModel = new SysRole()
            {
                RoleName = model.RoleName,
                Menus = model.Menus,
                CreateDate = DateTime.Now,
                CreateUserId = userId,
                IsDeleted = false
            };

            context.SysRoles.Add(newModel);
            context.SaveChanges();

            // 处理返回值
            model.Id = newModel.Id;
            model.RoleName = newModel.RoleName;
            model.Menus = newModel.Menus;

            // 记录日志
            logger.Info("添加角色：{0}", JsonConvert.SerializeObject(model));

            return Content<TempSysRole>(HttpStatusCode.Created, model);
        }

        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Put")]
        public TempSysRole Put(TempSysRole model)
        {
            // 通过token获取当前用户
            var userId = User.Identity.GetUserId();

            // 处理逻辑
            var oldModel = context.SysRoles.First(t => t.Id == model.Id);
            oldModel.RoleName = model.RoleName;
            oldModel.Menus = model.Menus;
            oldModel.UpdateDate = DateTime.Now;
            oldModel.UpdateUserId = userId;

            context.Entry(oldModel).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            // 处理返回值

            // 记录日志
            logger.Info("修改角色：{0}", JsonConvert.SerializeObject(model));

            return model;
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="Id"></param>
        [Route("Delete")]
        public TempSysRole Delete(int Id)
        {
            // 通过token获取当前用户
            var userId = User.Identity.GetUserId();

            // 处理逻辑
            var oldModel = context.SysRoles.First(t => t.Id == Id);
            oldModel.IsDeleted = true;
            oldModel.DeleteDate = DateTime.Now;
            oldModel.DeleteUserId = userId;
            context.Entry(oldModel).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            // 处理返回值
            var tempSysRole = new TempSysRole
            {
                Id = Id,
                RoleName = oldModel.RoleName,
                Menus = oldModel.Menus
            };

            // 记录日志
            logger.Warn("删除角色：{0}", JsonConvert.SerializeObject(tempSysRole));

            return tempSysRole;
        }

        /// <summary>
        /// 批量删除角色
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public string DeleteBatch(string idList)
        {
            using (var tran = context.Database.BeginTransaction())
            {
                try
                {
                    // 通过token获取当前用户
                    var userId = User.Identity.GetUserId();

                    // 处理逻辑
                    string[] ids = idList.Split(',');
                    foreach (var item in ids)
                    {
                        var id = int.Parse(item);
                        var oldModel = context.SysRoles.First(t => t.Id == id);
                        oldModel.IsDeleted = true;
                        oldModel.DeleteDate = DateTime.Now;
                        oldModel.DeleteUserId = userId;
                        context.Entry(oldModel).State = System.Data.Entity.EntityState.Modified;
                    }
                    context.SaveChanges();

                    // 必须调用commit，不然不会保存数据
                    tran.Commit();

                    // 记录日志
                    logger.Warn("批量删除角色：{0}", idList);

                    return idList;
                }
                catch (Exception ex)
                {
                    // 出错回滚
                    tran.Rollback();

                    // 记录日志
                    logger.Error(ex, "批量删除角色失败！{0}", idList);
                    throw;
                }
            }
        }




        //// GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<controller>/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        //[Route("GetRoles")]
        //public Object GetRoles(int page=0,int pageSize=10)
        //{
        //    IQueryable<SysRole> query;
        //}
    }

    public class TempSysRole
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 菜单
        /// </summary>
        public string Menus { get; set; }
    }
}