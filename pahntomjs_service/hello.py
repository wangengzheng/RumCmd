import web


urls=(
  '/(.*)','hello'
)


app= web.application(urls,globals())


class hello:
    def GET(self,name):
        if not name:
            name='world'
        return 'Hell0,'+name+' !'


if __name__ =="__main__":
    app.run()
