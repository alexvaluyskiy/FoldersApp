using FoldersApp.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.IO;

namespace FoldersApp.Core.Filters
{
    public class MyExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is FileAlreadyExistsException)
            {
                var validationException = (FileAlreadyExistsException)context.Exception;
                
                context.Result = new BadRequestObjectResult(new KeyValuePair<string, string>(typeof(FileAlreadyExistsException).Name, validationException.FolderName));
            }
            else if (context.Exception is FileNotFoundException)
            {
                var validationException = (FileNotFoundException)context.Exception;
                context.Result = new BadRequestObjectResult(new KeyValuePair<string, string>(typeof(FileNotFoundException).Name, validationException.FileName));
            }

            base.OnException(context);
        }
    }
}
