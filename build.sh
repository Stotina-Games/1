mkdir -v -p build
rm -rf build/web
mkdir -v -p build/web

godot -v --export-release "TestGame" ./build/web/index.html

export HACK_ENABLE_THREADS='       <script type="text/javascript">self.addEventListener("fetch",function(e){("only-if-cached"!==e.request.cache||"same-origin"===e.request.mode)&&e.respondWith(fetch(e.request).then(function(e){let t=new Headers(e.headers);t.set("Cross-Origin-Embedder-Policy","require-corp"),t.set("Cross-Origin-Opener-Policy","same-origin");let r=new Response(e.body,{status:e.status,statusText:e.statusText,headers:t});return r}).catch(function(e){console.error(e)}))});</script>'
echo $HACK_ENABLE_THREADS > ./build/web/hack-enable-threads.html
sed -i -e '/<head>/r ./build/web/hack-enable-threads.html' ./build/web/index.html