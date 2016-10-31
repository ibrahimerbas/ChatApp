var http = require("http");
var fs = require("fs");
var url = require("url");
var path = require("path");

var mimeTypes = {
    "html": "text/html",
    "jpeg": "image/jpeg",
    "jpg": "image/jpeg",
    "png": "image/png",
    "js": "text/javascript",
    "css": "text/css"};


http.createServer(function(request, response){
    var uri = url.parse(request.url).pathname;
    var filename = path.join(process.cwd(), uri);
    var mimeType = mimeTypes[path.extname(filename).split(".")[1]];
    var content = fs.readFileSync(filename);
    response.writeHead(200, mimeType);
    response.write(content);
    response.end();
    return;
}).listen(5000);