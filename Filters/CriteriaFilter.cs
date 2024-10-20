namespace TrilhaNetAzureDesafio.Filters
{
    using Microsoft.AspNetCore.Http;
    using System.Reflection;
    using System.Threading.Tasks;
    using System;

    public class CriteriaFilter
    {
        public Nullable<Int32> Page { get; set; }

        public String Nome { get; set; }

        public static ValueTask<CriteriaFilter> BindAsync(HttpContext context, ParameterInfo _)
        {
            ArgumentNullException.ThrowIfNull(context);

            String data = context.Request.Query["pagina"];
            var page = 1;
            var nome = String.Empty;

            if (Int32.TryParse(data, out Int32 number))
            {
                page = number;
            }

            data = context.Request.Query["nome"];

            if (!String.IsNullOrEmpty(data) && !String.IsNullOrWhiteSpace(data))
            {
                nome = data;
            }

            var result = new CriteriaFilter { Page = page, Nome = nome };
            return ValueTask.FromResult<CriteriaFilter>(result);
        }
    }
}
