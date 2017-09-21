## install webpy
```
    git clone git://github.com/webpy/webpy.git
    ln -s `pwd`/webpy/web .
    
    全局安装
    cd webpy
    python setup.py install
    
```

## install fonts
```
   //gbk乱码，也就是截图出现口等乱码
   apt-get xfonts-wqy //ubuntu
   
   //字体粗细不一致问题
   // 这个问题的原因是截图服务器上没有安装页面所需的字体，解决方案就是安装所需字体。如何在centos上安装字体?，这里需要注意的是，/usr/share/fonts这个目录是root权限，可以通过上传到其他目录然后移动过来。
      
   
   //我们到“c:\windows\fonts”中把我们需要的字体拷贝出来（这里我们拷贝了“Arial”、“Comic Sans MS”、“Georgia”、“Times New Roman”四种）
   [如何给CentOS安装字体库](http://jhonge.net/Home/Single/26626627)
   
```

## install phantomjs
```
   apt-get install phantomjs   
```


## python web

```
    sh start.sh
    or
    python index.py 8765
```
