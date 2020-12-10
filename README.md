# dockerwaiter

A small command line utility for waiting exit codes from multiple containers in a docker-compose scenarios.

![.NET Core](https://github.com/jmanuelcorral/dockerwaiter/workflows/.NET%20Core/badge.svg)

## Problem

Working with docker-compose scenarios, normally I use docker-compose for launching integration tests in a build pipeline, with this approach, you normally finish your docker-compose scenario when a container exits (for sample, you launch a docker-compose that launch a database, a webserver, and a container that performs E2E tests) If you want to split this "E2E tests containers" you probably won't launch in parallel this tests because you only can exit from docker-compose from one

## What this command do?

You can pass to dockerwait a list of containers, a docker-compose file, and It will wait until all the containers exited with correct exit (0) or early exit in case of fail (different of 0 Exit Code).

## Sample usage

``` 
> docker-compose -f docker-compose.integrationtests.yml up --build -d
> dockerwaiter -c sales_tests accounting_tests -d docker-compose.integrationtests.yml
```
With docker-compose you launch your integration tests, and inside this docker-compose, you will have 2 containers executing the integration tests of sales and accounting (in parallel)

dockerwaiter awaits to all your containers end (with exitcode 0) or breaks your integration testing session if once of them exits without this exit code.

## How to install

 At the moment, we have packaged the solution as a dotnet global tool. You only need dotnet core (2, 3 or NET5) installed in your machine and install as a global tool typing:

``` 
> dotnet tool install --global dockerwaiter
```


