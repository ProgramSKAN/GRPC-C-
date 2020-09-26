GRPC Reflection 
Used to know the services that GRPC server provides without knowing the proto files on the client.

for this install GRPC.Reflection nuget on server
https://github.com/grpc/grpc/blob/master/doc/csharp/server_reflection.md

now download https://github.com/ktr0731/evans/releases EVANS Windows CLI to see the GRPC Server protos without client protos.
> tools>commandline>developer powershell
> ls   :::::::see evans.exe
> show package
> package greet
> show service
> service GreetingService
> desc GreetingRequest
> desc Greeting
> call Greet
	first_name=> program
	last_name=> skan
	{
		"result"="Hello Program Skan"
	}
> 




