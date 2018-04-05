const express = require('express')
const app = express()
var port = process.env.PORT || 80;

app.use(express.static('public'));

app.listen(port, () => console.log("Server running at http://localhost:%d", port));

// app.get("/",function(request, response){
//    response.send("<h1>Hello World</h1>"); 
// });