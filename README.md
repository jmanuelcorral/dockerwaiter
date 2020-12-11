![.NET Core](https://github.com/jmanuelcorral/dockerwaiter/workflows/.NET%20Core/badge.svg) ![Nuget](https://img.shields.io/nuget/dt/dockerwaiter) ![Nuget](https://img.shields.io/nuget/v/dockerwaiter)

# dockerwaiter

A small command line utility for waiting exit codes from multiple containers in a docker-compose scenarios.

## Give a Star! :star:

If you like or are using this utility in your solution, please give it a star. Thanks!

## Motivation

When You Work with docker-compose scenarios normally You will have a docker-compose for launching integration tests in a build pipeline.

Normally you build your docker-compose with all of your scenario, and a container run your integration tests, when this tests are executed in order, when your container exits, you normally exit and release the rest of the docker-compose scenario (with the arguments ```--abort-on-container-exit``` and ```--exit-code-from <service_name>``` ). 

This is a common behaviour if you are doing integration testing, but... if you need to launch more than one integration test container? how do you do for waiting to all of this containers exit? If one of them fails how your scenario stops?


With dockerwait you can wait for a list of containers to exit, and if one of them fails, all stuff in the docker-compose is released and stopped.

## Sample usage

```bash
# you can launch your docker-compose in detached mode
> docker-compose -f docker-compose.integrationtests.yml up --build -d
# whit this you can await your containers
> dockerwaiter -c sales_tests accounting_tests -d docker-compose.integrationtests.yml
```
With docker-compose you launch your integration tests, and inside this docker-compose, you will have 2 containers executing the integration tests of sales and accounting (they are doing their testing stuff in parallel).

dockerwaiter awaits to all your containers end (with exitcode 0) or breaks your integration testing session if once of them exits without this exit code.

Also you can setup a timeout 😎, by default is one hour.

## How to install

 At the moment, we have packaged the solution as a dotnet global tool. You only need dotnet core (2, 3 or NET5) installed in your machine and install as a global tool typing:

``` 
> dotnet tool install --global dockerwaiter
```


