### 运行命令行 通过 csharp code

# Code

``` 
        //借用 wkhtmltopdf 将网址生成pdf        
 	var cmd = new RunCmd();
	
	cmd.ToolPath=@"E:\1nuget\wkhtmltopdf-x64.0.13.0-alpha3\tools\wkhtmltopdf";
	cmd.ExeName =@"wkhtmltopdf.exe";	
	cmd.SaveFilesPath=@"E:\1nuget\wkhtmltopdf-x64.0.13.0-alpha3\tools\wkhtmltopdf";
	
	cmd.CustomArgs ="http://baidu.com baidu.pdf";
	
	cmd.Run();
	
```

## Nuget

```
      nuget install wkhtmltopdf-x86-win32 --version 0.12.3.6
      or
      dotnet add package wkhtmltopdf-x86-win32 --version 0.12.3.6
      
      
```

官网 [wkhtmltopdf](http://wkhtmltopdf.org/) 
Nuget地址 [wkhtmltopdf](https://www.nuget.org/packages/wkhtmltopdf-x86-win32/)
参考 [NReco.PhantomJS](https://www.nuget.org/packages/NReco.PhantomJS/)


## 工具 LinqPad4
