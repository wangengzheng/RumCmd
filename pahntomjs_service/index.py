# -*- coding:utf-8 -*-

import web
from web.httpserver import StaticMiddleware
from handlers import (WebimgHandler)

__author__ = 'wait'

urls = (
    '/webimg', WebimgHandler,
)



app = web.application(urls, globals())
app.wsgifunc(StaticMiddleware)
web.config.debug = False
if '__main__' == __name__:
    app.run()
