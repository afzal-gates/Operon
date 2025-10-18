using Operon.Application.Common.ViewModels;
using System.Net;
using System.Text.Json;

namespace Operon.Application.Extensions
{
    public static class ResponseModelExtensions
    {
        public static async Task<ResponseViewModel> AsSuccess<T>(this Task<T> response)
        {
            if (response is not null && !response.IsFaulted)
            {
                return new SuccessResponseViewModel
                {
                    Data = await response
                };
            }

            throw response.Exception.InnerException;
        }

        public static ResponseViewModel AsSuccess<T>(this T response)
        {
            if (response is not null)
            {
                return new SuccessResponseViewModel
                {
                    Data = response
                };
            }
            return new FailResponseViewModel();
        }

        public static async Task<ResponseViewModel> AsCreated<T>(this Task<T> response)
        {
            if (response is not null && !response.IsFaulted)
            {
                return new SuccessResponseViewModel(message: HttpStatusCode.Created.ToString(), code: HttpStatusCode.Created)
                {
                    Data = await response
                };
            }

            throw response.Exception.InnerException;
        }

        public static async Task<ResponseViewModel> AsUpdated<T>(this Task<T> response)
        {
            if (response is not null && !response.IsFaulted)
            {
                return new SuccessResponseViewModel(message: "Updated", code: HttpStatusCode.OK)
                {
                    Data = await response
                };
            }

            throw response.Exception.InnerException;
        }

        public static async Task<ResponseViewModel> AsDeleted<T>(this Task<T> response)
        {
            if (response is not null && !response.IsFaulted)
            {
                return new SuccessResponseViewModel(message: "Deleted", code: HttpStatusCode.OK)
                {
                    Data = await response
                };
            }

            throw response.Exception.InnerException;
        }

        private static List<string> GetExceptions<T>(this Task<T> response)
        {
            List<string> exceptions = new();
            if (response == null) return exceptions;

            foreach (var ex in response.Exception?.InnerExceptions)
            {
                var message = ex.InnerException is null ? $"EXCEPTION ==> {ex.Message}" :
                    "EXCEPTION ==>" + ("(1). " + ex.Message + (" (2). " + ex.InnerException.Message)) +
                    (ex.InnerException?.InnerException is null ? "" : " (3). " + ex.InnerException.InnerException.Message);

                exceptions.Add(message);
            }

            return exceptions;
        }
    }
}
