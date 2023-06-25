using System.Net;

namespace App.ExtendMethods
{
    public static class AppExtends
    {
        public static void AddStatusCodePage(this WebApplication app)
        {
            app.UseStatusCodePages(appError =>
            {
                appError.Run(async context =>
                {
                    var statusCode = context.Response.StatusCode;

                    var content = @$"
                    <!DOCTYPE html>
                        <head>
                            <meta charset='UTF-8'>
                            <title>Lỗi: {statusCode}</title>
                        </head>
                        <body>
                            <h2 style='color:red;'>
                                Có lỗi xảy ra: {statusCode} - {(HttpStatusCode)statusCode}
                            </h2>
                        </body>
                        </html>
                    ";

                    await context.Response.WriteAsync(content);
                });
            });
        }
    }
}