<Query Kind="Program" />

void Main()
{
	var cmd = new RunCmd();
	
	cmd.ToolPath=@"E:\1nuget\wkhtmltopdf-x64.0.13.0-alpha3\tools\wkhtmltopdf";
	cmd.ExeName =@"wkhtmltopdf.exe";	
	cmd.SaveFilesPath=@"E:\1nuget\wkhtmltopdf-x64.0.13.0-alpha3\tools\wkhtmltopdf";
	
	cmd.CustomArgs ="http://baidu.com baidu.pdf";
	
	cmd.Run();
	
}

// Define other methods and classes here
public class RunCmd
{
    public RunCmd()
	{	
       this.ProcessPriority = ProcessPriorityClass.Normal;
	}

    private Process RumProcess =null;
	
    private List<string> errorLines = new List<string>();

    public string ToolPath { get; set; }
	 
	public string ExeName { get; set; }
	
	public string CustomArgs { get; set; }
	
	public ProcessPriorityClass ProcessPriority { get; set; }
	
	private string saveFilesPath{ get; set; }
	
	//文件保存地址
	public string SaveFilesPath {
	  get{
	      return this.saveFilesPath+"\\workTempPath";
	  }
	  set{
	     saveFilesPath =value;
	  }
	}
    	
	private string GetTempFilesPath()
    {	    	    
		if (!string.IsNullOrEmpty(this.SaveFilesPath) && !Directory.Exists(this.SaveFilesPath))
		{
			Directory.CreateDirectory(this.SaveFilesPath);		
		}
		return this.SaveFilesPath ?? Path.GetTempPath();
	}
	
	public TimeSpan? ExecutionTimeout { get; set; }
	
	/// <summary>
	/// Occurs when output data is received from PhantomJS process
	/// </summary>
	public event EventHandler<DataReceivedEventArgs> OutputReceived;

	/// <summary>
	/// Occurs when error data is received from PhantomJS process
	/// </summary>
    public event EventHandler<DataReceivedEventArgs> ErrorReceived;
	
		
	public void Run()
	{
	  Console.WriteLine ("run begin");
	   
	 try
	 {
	    RunInternal(null,null);
    	this.WaitProcessForExit();
		this.CheckExitCode(this.RumProcess.ExitCode, this.errorLines);
	 }
	 finally
	 {
		this.RumProcess.Close();
		this.RumProcess = null;
	 }
	   
	  Console.WriteLine ("run end");
	}
    
	private void RunInternal(Stream inputStream, Stream outputStream)
		{
			this.errorLines.Clear();
			try
			{
				string text = Path.Combine(this.ToolPath, this.ExeName);
				if (!File.Exists(text))
				{
					throw new FileNotFoundException("Cannot find ExeName: " + text);
				}
				StringBuilder stringBuilder = new StringBuilder();
				if (!string.IsNullOrEmpty(this.CustomArgs))
				{
					stringBuilder.AppendFormat(" {0} ", this.CustomArgs);
				}
				
				Console.WriteLine(stringBuilder.ToString().Trim());
				ProcessStartInfo processStartInfo = new ProcessStartInfo(text, stringBuilder.ToString());
				processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				//processStartInfo.WindowStyle = ProcessWindowStyle.Maximized;
				processStartInfo.CreateNoWindow = true;
				processStartInfo.UseShellExecute = false;
				processStartInfo.WorkingDirectory = Path.GetDirectoryName(this.SaveFilesPath);
				processStartInfo.RedirectStandardInput = true;
				processStartInfo.RedirectStandardOutput = true;
				processStartInfo.RedirectStandardError = true;
				this.RumProcess = new Process();
				this.RumProcess.StartInfo = processStartInfo;
				this.RumProcess.EnableRaisingEvents = true;
				this.RumProcess.Start();
				if (this.ProcessPriority != ProcessPriorityClass.Normal)
				{
					this.RumProcess.PriorityClass = this.ProcessPriority;
				}
				this.RumProcess.ErrorDataReceived += delegate(object o, DataReceivedEventArgs args)
				{
					if (args.Data == null)
					{
						return;
					}
					this.errorLines.Add(args.Data);
					if (this.ErrorReceived != null)
					{
						this.ErrorReceived(this, args);
					}
				};
				this.RumProcess.BeginErrorReadLine();
				if (outputStream == null)
				{
					this.RumProcess.OutputDataReceived += delegate(object o, DataReceivedEventArgs args)
					{
						if (this.OutputReceived != null)
						{
							this.OutputReceived(this, args);
						}
					};
					this.RumProcess.BeginOutputReadLine();
				}
				if (inputStream != null)
				{
					this.CopyToStdIn(inputStream);
				}
				if (outputStream != null)
				{
					this.ReadStdOutToStream(this.RumProcess, outputStream);
				}
			}
			catch (Exception ex)
			{
				this.EnsureProcessStopped();
				throw new Exception("Cannot execute ExeName: " + ex.Message, ex);
			}
		}
		
		
		private string PrepareCmdArg(string arg)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('"');
			stringBuilder.Append(arg.Replace("\"", "\\\""));
			stringBuilder.Append('"');
			return stringBuilder.ToString();
		}
		
		
		protected void CopyToStdIn(Stream inputStream)
		{
			byte[] array = new byte[8192];
			while (true)
			{
				int num = inputStream.Read(array, 0, array.Length);
				if (num <= 0)
				{
					break;
				}
				this.RumProcess.StandardInput.BaseStream.Write(array, 0, num);
				this.RumProcess.StandardInput.BaseStream.Flush();
			}
			this.RumProcess.StandardInput.Close();
		}
		
		private void ReadStdOutToStream(Process proc, Stream outputStream)
		{
			byte[] array = new byte[32768];
			int count;
			while ((count = proc.StandardOutput.BaseStream.Read(array, 0, array.Length)) > 0)
			{
				outputStream.Write(array, 0, count);
			}
		}
		
		private void EnsureProcessStopped()
		{
			if (this.RumProcess != null && !this.RumProcess.HasExited)
			{
				try
				{
					this.RumProcess.Kill();
					this.RumProcess.Dispose();
					this.RumProcess = null;
				}
				catch (Exception)
				{
				}
			}
		}
		
		private void WaitProcessForExit()
		{
			bool hasValue = this.ExecutionTimeout.HasValue;
			if (hasValue)
			{
				this.RumProcess.WaitForExit((int)this.ExecutionTimeout.Value.TotalMilliseconds);
			}
			else
			{
				this.RumProcess.WaitForExit();
			}
			if (this.RumProcess == null)
			{
				throw new RumProcessException(-1, "FFMpeg process was aborted");
			}
			if (hasValue && !this.RumProcess.HasExited)
			{
				this.EnsureProcessStopped();
				throw new RumProcessException(-2, string.Format("FFMpeg process exceeded execution timeout ({0}) and was aborted", this.ExecutionTimeout));
			}
		}
		
		private void CheckExitCode(int exitCode, List<string> errLines)
		{
			if (exitCode != 0)
			{
				throw new RumProcessException(exitCode, string.Join("\n", errLines.ToArray()));
			}
		}

		public void Dispose()
		{
			this.EnsureProcessStopped();
		}
		
		
		
		
}

	public class RumProcessException : Exception
	{
		/// <summary>
		/// Get WkHtmlToImage process error code
		/// </summary>
		public int ErrorCode
		{
			get;
			private set;
		}

		public RumProcessException(int errCode, string message)
			: base(string.Format("PhantomJS exit code {0}: {1}", errCode, message))
		{
			this.ErrorCode = errCode;
		}
	}
	