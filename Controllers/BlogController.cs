using Microsoft.AspNetCore.Mvc;
using Web_QLNhaHang.Models;

namespace Web_QLNhaHang.Controllers
{
    public class BlogController : Controller
    {
        private static readonly List<BlogPost> _posts = new List<BlogPost>
        {
            new BlogPost
            {
                Id = 1,
                Title = "Tinh hoa ẩm thực Phương Nam - Hương vị khó quên",
                Slug = "tinh-hoa-am-thuc-phuong-nam",
                Summary = "Khám phá nét độc đáo trong văn hóa ẩm thực miền Nam với những món ăn dân dã nhưng đậm đà bản sắc.",
                Content = @"<p>Ẩm thực phương Nam luôn có sức hút kỳ lạ đối với thực khách bởi sự phóng khoáng, dân dã nhưng không kém phần tinh tế...</p>
                            <p>Đến với Nhà hàng Việt, bạn sẽ được thưởng thức những món ăn chuẩn vị như Bánh xèo miền Tây, Lẩu cá kèo lá giang...</p>",
                ImageUrl = "/images/banners/hero-banner.png",
                PublishedDate = DateTime.Now.AddDays(-2)
            },
            new BlogPost
            {
                Id = 2,
                Title = "Top 5 món lẩu nhất định phải thử tại Nhà hàng Việt",
                Slug = "top-5-mon-lau-nhat-dinh-phai-thu",
                Summary = "Lẩu là món ăn không thể thiếu trong những buổi tụ tập. Cùng điểm qua 5 món lẩu 'best-seller' tại nhà hàng chúng tôi.",
                Content = @"<p>1. Lẩu gà lá giang: Vị chua thanh của lá giang kết hợp với thịt gà dai ngon...</p>
                            <p>2. Lẩu cá kèo: Đặc sản miền Tây sông nước...</p>",
                ImageUrl = "/images/dishes/lau-ca-keo.png",
                PublishedDate = DateTime.Now.AddDays(-5)
            },
            new BlogPost
            {
                Id = 3,
                Title = "Bí quyết làm món Cá kèo nướng muối ớt thơm ngon",
                Slug = "bi-quyet-lam-mon-ca-keo-nuong",
                Summary = "Học cách chế biến món cá kèo nướng muối ớt cay nồng, giòn rụm ngay tại nhà với công thức đơn giản.",
                Content = @"<p>Cá kèo là loại cá đặc sản, thịt ngọt, xương mềm...</p>",
                ImageUrl = "/images/dishes/ca-keo-muoi-ot.png",
                PublishedDate = DateTime.Now.AddDays(-10)
            }
        };

        public IActionResult Index()
        {
            return View(_posts);
        }

        public IActionResult Details(int id)
        {
            var post = _posts.FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }
    }
}
