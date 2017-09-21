# -*- coding:utf-8 -*-

import os
from os.path import (basename, splitext)
import web
import json
import hashlib
import datetime

__author__ = 'wait'


render = web.template.render('template/')


class WebimgHandler:
    def POST(self):
        param = json.loads(web.data())

        return json.dumps({'code': 0,
                           'data': web.ctx.homedomain + self.web2archive(param)
                           })

    def GET(self):
        print(web.input())
        if 'url' not in web.input():
            return web.notfound()

        # cache 6 hours
        #web.http.expires(21600)
	print('realpath:',os.path.realpath(__file__))
        filepath = os.path.dirname(os.path.realpath(__file__)) +'/'+ self.web2archive(web.input())
	print(filepath)
        #fmtime = datetime.datetime.fromtimestamp(os.path.getmtime(filepath))
	#print(fmtime)
        #etag = splitext(basename(filepath))[0]
        #web.http.modified(fmtime, etag)
	try:
            with open(filepath, 'r') as f:
                return f.read()
        except:
	    return "# you can send an 404 error page"


    def web2archive(self, param):
        cmd = 'phantomjs --debug=true rasterize.js'
         
	#cmd = 'phantomjs rasterize.js' 

        md5 = hashlib.md5()

        md5.update((param['url']+param['size']) if 'size' in param else param['url'])
        
        
        filepath = 'static/target/' + md5.hexdigest()

        print(filepath)

        cmd += ' ' + param['url']

        format = 'png'
        if 'format' in param:
            format = param['format']
        filepath += '.' + format
        web.header('Content-Type', 'image/' + format)

        # download file
        if 'download' in param:
            web.header('Content-disposition', 'attachment; filename=' + basename(filepath))


        if (os.path.isfile(filepath) and
                ('force' not in param or not param['force'])):
            print('path is file')
            return filepath

        cmd += ' ' + filepath

        if 'size' in param:
            cmd += ' ' + param['size']

        print(cmd)
        os.system(cmd)
        return filepath
