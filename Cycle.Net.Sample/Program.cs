﻿using System;
using Cycle.Net.Run;
using Cycle.Net.Run.Abstract;
using System.Reactive.Linq;
using System.Reactive.Concurrency;

namespace Cycle.Net.Sample
{
    class Program
    {
        static readonly IDriver[] Drivers = new[]
        {
            new HttpDriver(DefaultScheduler.Instance)
        };

        static void Main(string[] args)
        {
            new CycleNet<SimpleSource>(new SimpleSource(Drivers)).Run(Flow);
            Console.Read();
        }

        static readonly HttpResponse InitialResponse = new HttpResponse
        {
            Origin = new HttpRequest
            {
                Id = "init",
                Url = "init"
            },
            Content = "init"
        };

        static IObservable<IRequest> Flow(SimpleSource source)
        {
            var httpSource = source.GetDriver(HttpDriver.ID)
                .OfType<HttpResponse>()
                .StartWith(InitialResponse);

            var firstStep = httpSource.Where(response => response == InitialResponse)
                .Do(response => Console.WriteLine("fetching posts"))
                .Select(response => new HttpRequest
                {
                    Id = "posts",
                    Url = "https://jsonplaceholder.typicode.com/posts"
                });

            var secondStep = httpSource.Where(response => response.Origin.Id == "posts")
                .Do(response => Console.WriteLine("posts fetched, fetching users"))
                .Select(response => new HttpRequest
                {
                    Id = "users",
                    Url = "https://jsonplaceholder.typicode.com/users"
                });

            var thirdStep = httpSource.Where(response => response.Origin.Id == "users")
                .Do(response => Console.WriteLine("users fetched, fetching comments"))
                .Select(response => new HttpRequest
                {
                    Id = "comments",
                    Url = "https://jsonplaceholder.typicode.com/comments"
                });

            var lastStep = httpSource.Where(response => response.Origin.Id == "comments")
                .Do(response => Console.WriteLine("comments fetched"))
                .Select(response => EmptyRequest.Instance);

            return Observable.Merge<IRequest>(firstStep, secondStep, thirdStep, lastStep);
        }
    }
}
