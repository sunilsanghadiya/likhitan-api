﻿using likhitan.Common.Services;
using likhitan_api.Models;
using likhitan_api.Models.ClientDto;
using likhitan_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace likhitan.Controllers
{
    [Route("api/blog")]
    [ApiController]
    [Authorize]
    public class BlogController
    {
        private IBlogService _blogService;

        public BlogController(IBlogService blogService) 
        {
            _blogService = blogService;
        }


        [HttpPost("WriteBlog")]
        public async Task<Result<WriteBlogResponse>> WriteBlog([FromBody] WriteBlogDto writeBlogDto) =>
            await _blogService.WriteBlog(writeBlogDto);

        [HttpPost("GetBlogs")]
        public async Task<Result<IEnumerable<BlogsResponse>>> GetBlogs(GetBlogsDto getBlogsDto) =>
            await _blogService.GetBlogs(getBlogsDto);

        [HttpPost("BlogAction")]
        public async Task<Result<BlogActionResponse>> BlogAction(BlogActionDto blogActionDto) =>
            await _blogService.BlogAction(blogActionDto);
    }
}
