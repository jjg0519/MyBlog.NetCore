using System.Linq;
using System.Collections;
using System.Collections.Generic;

using MySqlSugar;
using MyBlog.Models;


namespace MyBlog.Core.ViewProjections.Home
{
    /// <summary>
    /// 标签所有文章视图投影类
    /// </summary>
    public class TagPostsViewProjection : IViewProjection<TagPostsBindModel, TagPostsViewModel>
    {
        private readonly IDbSession _db;


        public TagPostsViewProjection(IDbSession db)
        {
            this._db = db;
        }

        /// <summary>
        /// 投影数据到视图模型
        /// </summary>
        /// <param name="input">标签所有文章查询数据</param>
        /// <returns>标签所有文章详情视图模型</returns>
        public TagPostsViewModel Project(TagPostsBindModel input)
        {
            if (null == input || null == input.TagName)
                return new TagPostsViewModel()
                {
                    AllPageNum = -1,
                    PageNum = input.PageNum,
                    Posts = null,
                    TagName = input.TagName
                };

            // 查询出总页数


            // 查询出所有匹配的数据集合
            var queryPostTagList = this._db.GetSession().Queryable<post_tag_tb>(DbTableNames.post_tag_tb)
                .Where(pt => pt.tag_name == input.TagName).ToList();

            // 查询出所有匹配的数据总数
            var allCount = queryPostTagList.Count();
            var allPageNum = allCount / input.Take;
            if (allCount % input.Take != 0)
                allPageNum++;


            // 跳过的数据量
            var skip = (input.PageNum - 1) * input.Take;

            #region 未用的

            //// 创建子查询，获取所有post集合
            //var childQuery = this._db.GetSession().Queryable<post_tag_tb>(DbTableNames.post_tag_tb)
            //    .Where(pt => pt.tag_name == input.TagName)
            //    .Select(pt => new { post_id = pt.post_id })
            //    .ToSql();
            //// 将SQL语句用()包成表
            //string childTableName = SqlSugarTool.PackagingSQL(childQuery.Key);
            //// 
            //this._db.GetSession().Queryable<post_tb>(DbTableNames.post_tb)


            #endregion


            /*
                1、查询出所有包含查询标签的post_id临时表
                2、查询出post表中所有存在于post_id临时表中的post集合
            */

            // sql语句的写法
            //var queryTagPostList = this._db.GetSession().SqlQuery<post_tb>($"select post_id,post_title,post_summary,post_tags,post_path,post_pub_state,post_pub_time from post_tb where post_pub_state=1 and post_pub_time!='' and post_id in (select post_id from post_tag_tb where tag_name='{input.TagName}')")
            //    .OrderBydescending(p => p.post_pub_time)
            //    .Skip(skip)
            //    .Take(input.Take);

            var queryTagPostList = this._db.GetSession().SqlQuery<post_tb>($"select post_id,post_title,post_summary,post_tags,post_path,post_pub_state,post_pub_time from post_tb where post_pub_state=1 and post_pub_time!='' and post_id in (select post_id from post_tag_tb where tag_name='{input.TagName}')")
                .OrderByDescending(p => p.post_pub_time)
                .Skip(skip)
                .Take(input.Take);






            // 若未查询到数据则删除这个标签
            if (queryTagPostList.Count() <= 0)
            {
                //this._db.GetDbSession().Delete(new tag_tb() { tag_name = input.TagName });
            }


            return new TagPostsViewModel()
            {
                AllPageNum = allPageNum,
                PageNum = input.PageNum,
                Posts = queryTagPostList,
                TagName = input.TagName
            };

        }
    }
}
