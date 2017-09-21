# 使用说明

```
 	var cmd = new RunCmd();
	
	cmd.ToolPath=@"E:\1nuget\wkhtmltopdf-x64.0.13.0-alpha3\tools\wkhtmltopdf";
	cmd.ExeName =@"wkhtmltopdf.exe";	
	cmd.SaveFilesPath=@"E:\1nuget\wkhtmltopdf-x64.0.13.0-alpha3\tools\wkhtmltopdf";
	
	cmd.CustomArgs ="http://sj.edsmall.cn/SubMall/orderdetailpdfview.aspx?orderid=62f52d49-f597-4f36-b868-3678d0271a0b orderdetailv2.pdf";
	
	cmd.Run();
	
```