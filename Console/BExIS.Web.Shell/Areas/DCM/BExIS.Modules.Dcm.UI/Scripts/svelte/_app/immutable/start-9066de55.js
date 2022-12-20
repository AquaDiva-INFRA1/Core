import{S as ot,i as st,s as it,a as lt,e as F,c as ct,b as z,g as he,t as B,d as me,f as J,h as G,j as ft,o as Pe,k as ut,l as dt,m as pt,n as ke,p as V,q as ht,r as mt,u as _t,v as W,w as Y,x as Ne,y as X,z as Z,A as ce}from"./chunks/index-c0dba9d5.js";import{S as rt,I as C,g as We,f as Ye,a as Se,b as fe,s as M,i as Xe,c as pe,P as Ze,d as gt,e as wt,h as yt}from"./chunks/singletons-2ff65960.js";function bt(r,e){return r==="/"||e==="ignore"?r:e==="never"?r.endsWith("/")?r.slice(0,-1):r:e==="always"&&!r.endsWith("/")?r+"/":r}function vt(r){return r.split("%25").map(decodeURI).join("%25")}function Et(r){for(const e in r)r[e]=decodeURIComponent(r[e]);return r}const kt=["href","pathname","search","searchParams","toString","toJSON"];function St(r,e){const n=new URL(r);for(const i of kt){let s=n[i];Object.defineProperty(n,i,{get(){return e(),s},enumerable:!0,configurable:!0})}return Rt(n),n}function Rt(r){Object.defineProperty(r,"hash",{get(){throw new Error("Cannot access event.url.hash. Consider using `$page.url.hash` inside a component instead")}})}const Ot="/__data.json";function It(r){return r.replace(/\/$/,"")+Ot}function Lt(r){let e=5381;if(typeof r=="string"){let n=r.length;for(;n;)e=e*33^r.charCodeAt(--n)}else if(ArrayBuffer.isView(r)){const n=new Uint8Array(r.buffer,r.byteOffset,r.byteLength);let i=n.length;for(;i;)e=e*33^n[--i]}else throw new TypeError("value must be a string or TypedArray");return(e>>>0).toString(36)}const _e=window.fetch;window.fetch=(r,e)=>((r instanceof Request?r.method:(e==null?void 0:e.method)||"GET")!=="GET"&&te.delete(Ue(r)),_e(r,e));const te=new Map;function Pt(r,e){const n=Ue(r,e),i=document.querySelector(n);if(i!=null&&i.textContent){const{body:s,...u}=JSON.parse(i.textContent),t=i.getAttribute("data-ttl");return t&&te.set(n,{body:s,init:u,ttl:1e3*Number(t)}),Promise.resolve(new Response(s,u))}return _e(r,e)}function $t(r,e,n){if(te.size>0){const i=Ue(r,n),s=te.get(i);if(s){if(performance.now()<s.ttl&&["default","force-cache","only-if-cached",void 0].includes(n==null?void 0:n.cache))return new Response(s.body,s.init);te.delete(i)}}return _e(e,n)}function Ue(r,e){let i=`script[data-sveltekit-fetched][data-url=${JSON.stringify(r instanceof Request?r.url:r)}]`;return e!=null&&e.body&&(typeof e.body=="string"||ArrayBuffer.isView(e.body))&&(i+=`[data-hash="${Lt(e.body)}"]`),i}const At=/^(\[)?(\.\.\.)?(\w+)(?:=(\w+))?(\])?$/;function jt(r){const e=[];return{pattern:r==="/"?/^\/$/:new RegExp(`^${Ut(r).map(i=>{const s=/^\[\.\.\.(\w+)(?:=(\w+))?\]$/.exec(i);if(s)return e.push({name:s[1],matcher:s[2],optional:!1,rest:!0,chained:!0}),"(?:/(.*))?";const u=/^\[\[(\w+)(?:=(\w+))?\]\]$/.exec(i);if(u)return e.push({name:u[1],matcher:u[2],optional:!0,rest:!1,chained:!0}),"(?:/([^/]+))?";if(!i)return;const t=i.split(/\[(.+?)\](?!\])/);return"/"+t.map((_,h)=>{if(h%2){if(_.startsWith("x+"))return Re(String.fromCharCode(parseInt(_.slice(2),16)));if(_.startsWith("u+"))return Re(String.fromCharCode(..._.slice(2).split("-").map($=>parseInt($,16))));const g=At.exec(_);if(!g)throw new Error(`Invalid param: ${_}. Params and matcher names can only have underscores and alphanumeric characters.`);const[,y,S,U,T]=g;return e.push({name:U,matcher:T,optional:!!y,rest:!!S,chained:S?h===1&&t[0]==="":!1}),S?"(.*?)":y?"([^/]*)?":"([^/]+?)"}return Re(_)}).join("")}).join("")}/?$`),params:e}}function Nt(r){return!/^\([^)]+\)$/.test(r)}function Ut(r){return r.slice(1).split("/").filter(Nt)}function Tt(r,e,n){const i={},s=r.slice(1);let u="";for(let t=0;t<e.length;t+=1){const f=e[t];let _=s[t];if(f.chained&&f.rest&&u&&(_=_?u+"/"+_:u),u="",_===void 0)f.rest&&(i[f.name]="");else{if(f.matcher&&!n[f.matcher](_)){if(f.optional&&f.chained){let h=s.indexOf(void 0,t);if(h===-1){const g=e[t+1];if(g!=null&&g.rest&&g.chained)u=_;else return}for(;h>=t;)s[h]=s[h-1],h-=1;continue}return}i[f.name]=_}}if(!u)return i}function Re(r){return r.normalize().replace(/[[\]]/g,"\\$&").replace(/%/g,"%25").replace(/\//g,"%2[Ff]").replace(/\?/g,"%3[Ff]").replace(/#/g,"%23").replace(/[.*+?^${}()|\\]/g,"\\$&")}function Dt(r,e,n,i){const s=new Set(e);return Object.entries(n).map(([f,[_,h,g]])=>{const{pattern:y,params:S}=jt(f),U={id:f,exec:T=>{const $=y.exec(T);if($)return Tt($,S,i)},errors:[1,...g||[]].map(T=>r[T]),layouts:[0,...h||[]].map(t),leaf:u(_)};return U.errors.length=U.layouts.length=Math.max(U.errors.length,U.layouts.length),U});function u(f){const _=f<0;return _&&(f=~f),[_,r[f]]}function t(f){return f===void 0?f:[s.has(f),r[f]]}}function Vt(r){let e,n,i;var s=r[0][0];function u(t){return{props:{data:t[2],form:t[1]}}}return s&&(e=W(s,u(r))),{c(){e&&Y(e.$$.fragment),n=F()},l(t){e&&Ne(e.$$.fragment,t),n=F()},m(t,f){e&&X(e,t,f),z(t,n,f),i=!0},p(t,f){const _={};if(f&4&&(_.data=t[2]),f&2&&(_.form=t[1]),s!==(s=t[0][0])){if(e){he();const h=e;B(h.$$.fragment,1,0,()=>{Z(h,1)}),me()}s?(e=W(s,u(t)),Y(e.$$.fragment),J(e.$$.fragment,1),X(e,n.parentNode,n)):e=null}else s&&e.$set(_)},i(t){i||(e&&J(e.$$.fragment,t),i=!0)},o(t){e&&B(e.$$.fragment,t),i=!1},d(t){t&&G(n),e&&Z(e,t)}}}function Ct(r){let e,n,i;var s=r[0][0];function u(t){return{props:{data:t[2],$$slots:{default:[qt]},$$scope:{ctx:t}}}}return s&&(e=W(s,u(r))),{c(){e&&Y(e.$$.fragment),n=F()},l(t){e&&Ne(e.$$.fragment,t),n=F()},m(t,f){e&&X(e,t,f),z(t,n,f),i=!0},p(t,f){const _={};if(f&4&&(_.data=t[2]),f&523&&(_.$$scope={dirty:f,ctx:t}),s!==(s=t[0][0])){if(e){he();const h=e;B(h.$$.fragment,1,0,()=>{Z(h,1)}),me()}s?(e=W(s,u(t)),Y(e.$$.fragment),J(e.$$.fragment,1),X(e,n.parentNode,n)):e=null}else s&&e.$set(_)},i(t){i||(e&&J(e.$$.fragment,t),i=!0)},o(t){e&&B(e.$$.fragment,t),i=!1},d(t){t&&G(n),e&&Z(e,t)}}}function qt(r){let e,n,i;var s=r[0][1];function u(t){return{props:{data:t[3],form:t[1]}}}return s&&(e=W(s,u(r))),{c(){e&&Y(e.$$.fragment),n=F()},l(t){e&&Ne(e.$$.fragment,t),n=F()},m(t,f){e&&X(e,t,f),z(t,n,f),i=!0},p(t,f){const _={};if(f&8&&(_.data=t[3]),f&2&&(_.form=t[1]),s!==(s=t[0][1])){if(e){he();const h=e;B(h.$$.fragment,1,0,()=>{Z(h,1)}),me()}s?(e=W(s,u(t)),Y(e.$$.fragment),J(e.$$.fragment,1),X(e,n.parentNode,n)):e=null}else s&&e.$set(_)},i(t){i||(e&&J(e.$$.fragment,t),i=!0)},o(t){e&&B(e.$$.fragment,t),i=!1},d(t){t&&G(n),e&&Z(e,t)}}}function Qe(r){let e,n=r[5]&&xe(r);return{c(){e=ut("div"),n&&n.c(),this.h()},l(i){e=dt(i,"DIV",{id:!0,"aria-live":!0,"aria-atomic":!0,style:!0});var s=pt(e);n&&n.l(s),s.forEach(G),this.h()},h(){ke(e,"id","svelte-announcer"),ke(e,"aria-live","assertive"),ke(e,"aria-atomic","true"),V(e,"position","absolute"),V(e,"left","0"),V(e,"top","0"),V(e,"clip","rect(0 0 0 0)"),V(e,"clip-path","inset(50%)"),V(e,"overflow","hidden"),V(e,"white-space","nowrap"),V(e,"width","1px"),V(e,"height","1px")},m(i,s){z(i,e,s),n&&n.m(e,null)},p(i,s){i[5]?n?n.p(i,s):(n=xe(i),n.c(),n.m(e,null)):n&&(n.d(1),n=null)},d(i){i&&G(e),n&&n.d()}}}function xe(r){let e;return{c(){e=ht(r[6])},l(n){e=mt(n,r[6])},m(n,i){z(n,e,i)},p(n,i){i&64&&_t(e,n[6])},d(n){n&&G(e)}}}function Ft(r){let e,n,i,s,u;const t=[Ct,Vt],f=[];function _(g,y){return g[0][1]?0:1}e=_(r),n=f[e]=t[e](r);let h=r[4]&&Qe(r);return{c(){n.c(),i=lt(),h&&h.c(),s=F()},l(g){n.l(g),i=ct(g),h&&h.l(g),s=F()},m(g,y){f[e].m(g,y),z(g,i,y),h&&h.m(g,y),z(g,s,y),u=!0},p(g,[y]){let S=e;e=_(g),e===S?f[e].p(g,y):(he(),B(f[S],1,1,()=>{f[S]=null}),me(),n=f[e],n?n.p(g,y):(n=f[e]=t[e](g),n.c()),J(n,1),n.m(i.parentNode,i)),g[4]?h?h.p(g,y):(h=Qe(g),h.c(),h.m(s.parentNode,s)):h&&(h.d(1),h=null)},i(g){u||(J(n),u=!0)},o(g){B(n),u=!1},d(g){f[e].d(g),g&&G(i),h&&h.d(g),g&&G(s)}}}function Bt(r,e,n){let{stores:i}=e,{page:s}=e,{components:u}=e,{form:t}=e,{data_0:f=null}=e,{data_1:_=null}=e;ft(i.page.notify);let h=!1,g=!1,y=null;return Pe(()=>{const S=i.page.subscribe(()=>{h&&(n(5,g=!0),n(6,y=document.title||"untitled page"))});return n(4,h=!0),S}),r.$$set=S=>{"stores"in S&&n(7,i=S.stores),"page"in S&&n(8,s=S.page),"components"in S&&n(0,u=S.components),"form"in S&&n(1,t=S.form),"data_0"in S&&n(2,f=S.data_0),"data_1"in S&&n(3,_=S.data_1)},r.$$.update=()=>{r.$$.dirty&384&&i.page.set(s)},[u,t,f,_,h,g,y,i,s]}class Jt extends ot{constructor(e){super(),st(this,e,Bt,Ft,it,{stores:7,page:8,components:0,form:1,data_0:2,data_1:3})}}const Gt="modulepreload",Kt=function(r,e){return new URL(r,e).href},et={},q=function(e,n,i){if(!n||n.length===0)return e();const s=document.getElementsByTagName("link");return Promise.all(n.map(u=>{if(u=Kt(u,i),u in et)return;et[u]=!0;const t=u.endsWith(".css"),f=t?'[rel="stylesheet"]':"";if(!!i)for(let g=s.length-1;g>=0;g--){const y=s[g];if(y.href===u&&(!t||y.rel==="stylesheet"))return}else if(document.querySelector(`link[href="${u}"]${f}`))return;const h=document.createElement("link");if(h.rel=t?"stylesheet":Gt,t||(h.as="script",h.crossOrigin=""),h.href=u,document.head.appendChild(h),t)return new Promise((g,y)=>{h.addEventListener("load",g),h.addEventListener("error",()=>y(new Error(`Unable to preload CSS for ${u}`)))})})).then(()=>e())},Mt={},ge=[()=>q(()=>import("./chunks/0-7c6d9d2c.js"),["./chunks\\0-7c6d9d2c.js","./chunks\\_layout-7e4796dc.js","./components\\pages\\_layout.svelte-1c0dc403.js","./chunks\\index-c0dba9d5.js"],import.meta.url),()=>q(()=>import("./chunks/1-867c775a.js"),["./chunks\\1-867c775a.js","./components\\error.svelte-7e4c908b.js","./chunks\\index-c0dba9d5.js","./chunks\\singletons-2ff65960.js","./chunks\\index-7d6e52d2.js"],import.meta.url),()=>q(()=>import("./chunks/2-1217abf3.js"),["./chunks\\2-1217abf3.js","./components\\pages\\_page.svelte-2b4e4457.js","./chunks\\index-c0dba9d5.js","./chunks\\Api-91c0bab6.js","./chunks\\index-7d6e52d2.js","./assets\\Api-e37aa675.css","./chunks\\FileUploader-10bc4e73.js","./assets\\FileUploader-af927694.css"],import.meta.url),()=>q(()=>import("./chunks/3-a8d0e482.js"),["./chunks\\3-a8d0e482.js","./components\\pages\\create\\_page.svelte-7d279c4f.js","./chunks\\index-c0dba9d5.js","./chunks\\Label-a65e89be.js","./chunks\\Spinner-f7729107.js","./assets\\Spinner-60614438.css","./assets\\FileUploader-af927694.css","./chunks\\Api-91c0bab6.js","./chunks\\index-7d6e52d2.js","./assets\\Api-e37aa675.css","./chunks\\vest.production-f8a8982b.js","./chunks\\EntityTemplateCaller-20bdef08.js","./chunks\\transitions-b69fac1e.js","./chunks\\BaseCaller-0756f2c9.js","./assets\\_page-ab194bdd.css"],import.meta.url),()=>q(()=>import("./chunks/4-5f278da2.js"),["./chunks\\4-5f278da2.js","./components\\pages\\edit\\_page.svelte-7f983247.js","./chunks\\index-c0dba9d5.js","./chunks\\Label-a65e89be.js","./chunks\\Spinner-f7729107.js","./assets\\Spinner-60614438.css","./assets\\FileUploader-af927694.css","./chunks\\Api-91c0bab6.js","./chunks\\index-7d6e52d2.js","./assets\\Api-e37aa675.css","./chunks\\caller-7c91010f.js","./chunks\\Debug-4654932d.js","./chunks\\transitions-b69fac1e.js","./chunks\\index-e740ee3c.js","./chunks\\FileUploader-10bc4e73.js","./chunks\\BaseCaller-0756f2c9.js","./assets\\_page-eaae8431.css"],import.meta.url),()=>q(()=>import("./chunks/5-87057d55.js"),["./chunks\\5-87057d55.js","./components\\pages\\entitytemplate\\_page.svelte-a1875837.js","./chunks\\index-c0dba9d5.js","./chunks\\Label-a65e89be.js","./chunks\\Spinner-f7729107.js","./assets\\Spinner-60614438.css","./assets\\FileUploader-af927694.css","./chunks\\EntityTemplateCaller-20bdef08.js","./chunks\\transitions-b69fac1e.js","./chunks\\Api-91c0bab6.js","./chunks\\index-7d6e52d2.js","./assets\\Api-e37aa675.css","./chunks\\Container-089c5a99.js","./chunks\\Select-4879d919.js","./assets\\Select-5fc0187f.css","./chunks\\vest.production-f8a8982b.js","./chunks\\index-e740ee3c.js"],import.meta.url),()=>q(()=>import("./chunks/6-7585e52a.js"),["./chunks\\6-7585e52a.js","./components\\pages\\structuresuggestion\\_page.svelte-54ef841f.js","./chunks\\index-c0dba9d5.js","./chunks\\Select-4879d919.js","./chunks\\Spinner-f7729107.js","./assets\\Spinner-60614438.css","./assets\\Select-5fc0187f.css","./chunks\\Label-a65e89be.js","./assets\\FileUploader-af927694.css","./chunks\\caller-7c91010f.js","./chunks\\Api-91c0bab6.js","./chunks\\index-7d6e52d2.js","./assets\\Api-e37aa675.css","./chunks\\vest.production-f8a8982b.js","./chunks\\index-e740ee3c.js","./chunks\\BaseCaller-0756f2c9.js","./assets\\_page-21f09931.css"],import.meta.url),()=>q(()=>import("./chunks/7-2297ea06.js"),["./chunks\\7-2297ea06.js","./components\\pages\\view\\_page.svelte-c1650c22.js","./chunks\\index-c0dba9d5.js","./chunks\\Api-91c0bab6.js","./chunks\\index-7d6e52d2.js","./assets\\Api-e37aa675.css","./chunks\\Spinner-f7729107.js","./assets\\Spinner-60614438.css","./chunks\\Container-089c5a99.js","./chunks\\Debug-4654932d.js","./assets\\_page-e89cd081.css"],import.meta.url)],zt=[],Ht={"/":[2],"/create":[3],"/edit":[4],"/entitytemplate":[5],"/structuresuggestion":[6],"/view":[7]},Wt={handleError:({error:r})=>{console.error(r)}};class $e{constructor(e,n){this.status=e,typeof n=="string"?this.body={message:n}:n?this.body=n:this.body={message:`Error: ${e}`}}toString(){return JSON.stringify(this.body)}}class tt{constructor(e,n){this.status=e,this.location=n}}async function Yt(r){var e;for(const n in r)if(typeof((e=r[n])==null?void 0:e.then)=="function")return Object.fromEntries(await Promise.all(Object.entries(r).map(async([i,s])=>[i,await s])));return r}Object.getOwnPropertyNames(Object.prototype).sort().join("\0");Object.getOwnPropertyNames(Object.prototype).sort().join("\0");const Xt=-1,Zt=-2,Qt=-3,xt=-4,en=-5,tn=-6;function nn(r){if(typeof r=="number")return i(r,!0);if(!Array.isArray(r)||r.length===0)throw new Error("Invalid input");const e=r,n=Array(e.length);function i(s,u=!1){if(s===Xt)return;if(s===Qt)return NaN;if(s===xt)return 1/0;if(s===en)return-1/0;if(s===tn)return-0;if(u)throw new Error("Invalid input");if(s in n)return n[s];const t=e[s];if(!t||typeof t!="object")n[s]=t;else if(Array.isArray(t))if(typeof t[0]=="string")switch(t[0]){case"Date":n[s]=new Date(t[1]);break;case"Set":const _=new Set;n[s]=_;for(let y=1;y<t.length;y+=1)_.add(i(t[y]));break;case"Map":const h=new Map;n[s]=h;for(let y=1;y<t.length;y+=2)h.set(i(t[y]),i(t[y+1]));break;case"RegExp":n[s]=new RegExp(t[1],t[2]);break;case"Object":n[s]=Object(t[1]);break;case"BigInt":n[s]=BigInt(t[1]);break;case"null":const g=Object.create(null);n[s]=g;for(let y=1;y<t.length;y+=2)g[t[y]]=i(t[y+1]);break}else{const f=new Array(t.length);n[s]=f;for(let _=0;_<t.length;_+=1){const h=t[_];h!==Zt&&(f[_]=i(h))}}else{const f={};n[s]=f;for(const _ in t){const h=t[_];f[_]=i(h)}}return n[s]}return i(0)}const Oe=Dt(ge,zt,Ht,Mt),Ae=ge[0],je=ge[1];Ae();je();let ne={};try{ne=JSON.parse(sessionStorage[rt])}catch{}function Ie(r){ne[r]=pe()}function rn({target:r,base:e}){var Me;const n=document.documentElement,i=[];let s=null;const u={before_navigate:[],after_navigate:[]};let t={branch:[],error:null,url:null},f=!1,_=!1,h=!0,g=!1,y=!1,S=!1,U=!1,T,$=(Me=history.state)==null?void 0:Me[C];$||($=Date.now(),history.replaceState({...history.state,[C]:$},"",location.href));const we=ne[$];we&&(history.scrollRestoration="manual",scrollTo(we.x,we.y));let K,Te,re;async function De(){re=re||Promise.resolve(),await re,re=null;const a=new URL(location.href),o=se(a,!0);s=null,await Ce(o,a,[])}async function ye(a,{noScroll:o=!1,replaceState:c=!1,keepFocus:l=!1,state:p={},invalidateAll:d=!1},m,v){return typeof a=="string"&&(a=new URL(a,We(document))),ie({url:a,scroll:o?pe():null,keepfocus:l,redirect_chain:m,details:{state:p,replaceState:c},nav_token:v,accepted:()=>{d&&(U=!0)},blocked:()=>{},type:"goto"})}async function Ve(a){const o=se(a,!1);if(!o)throw new Error(`Attempted to preload a URL that does not belong to this app: ${a}`);return s={id:o.id,promise:Be(o).then(c=>(c.type==="loaded"&&c.state.error&&(s=null),c))},s.promise}async function ae(...a){const c=Oe.filter(l=>a.some(p=>l.exec(p))).map(l=>Promise.all([...l.layouts,l.leaf].map(p=>p==null?void 0:p[1]())));await Promise.all(c)}async function Ce(a,o,c,l,p={},d){var v,b;Te=p;let m=a&&await Be(a);if(m||(m=await Ke(o,{id:null},await ee(new Error(`Not found: ${o.pathname}`),{url:o,params:{},route:{id:null}}),404)),o=(a==null?void 0:a.url)||o,Te!==p)return!1;if(m.type==="redirect")if(c.length>10||c.includes(o.pathname))m=await oe({status:500,error:await ee(new Error("Redirect loop"),{url:o,params:{},route:{id:null}}),url:o,route:{id:null}});else return ye(new URL(m.location,o).href,{},[...c,o.pathname],p),!1;else((b=(v=m.props)==null?void 0:v.page)==null?void 0:b.status)>=400&&await M.updated.check()&&await le(o);if(i.length=0,U=!1,g=!0,l&&l.details){const{details:w}=l,O=w.replaceState?0:1;w.state[C]=$+=O,history[w.replaceState?"replaceState":"pushState"](w.state,"",o)}if(s=null,_){t=m.state,m.props.page&&(m.props.page.url=o);const w=de();T.$set(m.props),w()}else qe(m);if(l){const{scroll:w,keepfocus:O}=l;if(O||Le(),await ce(),h){const I=o.hash&&document.getElementById(o.hash.slice(1));w?scrollTo(w.x,w.y):I?I.scrollIntoView():scrollTo(0,0)}}else await ce();h=!0,m.props.page&&(K=m.props.page),d&&d(),g=!1}function qe(a){var p;t=a.state;const o=document.querySelector("style[data-sveltekit]");o&&o.remove(),K=a.props.page;const c=de();T=new Jt({target:r,props:{...a.props,stores:M},hydrate:!0}),c();const l={from:null,to:ue("to",{params:t.params,route:{id:((p=t.route)==null?void 0:p.id)??null},url:new URL(location.href)}),willUnload:!1,type:"enter"};u.after_navigate.forEach(d=>d(l)),_=!0}async function Q({url:a,params:o,branch:c,status:l,error:p,route:d,form:m}){const v=c.filter(Boolean);let b="never";for(const R of c)(R==null?void 0:R.slash)!==void 0&&(b=R.slash);a.pathname=bt(a.pathname,b),a.search=a.search;const w={type:"loaded",state:{url:a,params:o,branch:c,error:p,route:d},props:{components:v.map(R=>R.node.component)}};m!==void 0&&(w.props.form=m);let O={},I=!K;for(let R=0;R<v.length;R+=1){const E=v[R];O={...O,...E.data},(I||!t.branch.some(A=>A===E))&&(w.props[`data_${R}`]=O,I=I||Object.keys(E.data??{}).length>0)}if(I||(I=Object.keys(K.data).length!==Object.keys(O).length),!t.url||a.href!==t.url.href||t.error!==p||m!==void 0||I){w.props.page={error:p,params:o,route:d,status:l,url:new URL(a),form:m??null,data:I?O:K.data},Object.defineProperty(w.props.page,"routeId",{get(){throw new Error("$page.routeId has been replaced by $page.route.id")},enumerable:!1});const R=(E,A)=>{Object.defineProperty(w.props.page,E,{get:()=>{throw new Error(`$page.${E} has been replaced by $page.url.${A}`)}})};R("origin","origin"),R("path","pathname"),R("query","searchParams")}return w}async function be({loader:a,parent:o,url:c,params:l,route:p,server_data_node:d}){var w,O,I;let m=null;const v={dependencies:new Set,params:new Set,parent:!1,route:!1,url:!1},b=await a();if((w=b.universal)!=null&&w.load){let D=function(...E){for(const A of E){const{href:N}=new URL(A,c);v.dependencies.add(N)}};const R={route:{get id(){return v.route=!0,p.id}},params:new Proxy(l,{get:(E,A)=>(v.params.add(A),E[A])}),data:(d==null?void 0:d.data)??null,url:St(c,()=>{v.url=!0}),async fetch(E,A){let N;E instanceof Request?(N=E.url,A={body:E.method==="GET"||E.method==="HEAD"?void 0:await E.blob(),cache:E.cache,credentials:E.credentials,headers:E.headers,integrity:E.integrity,keepalive:E.keepalive,method:E.method,mode:E.mode,redirect:E.redirect,referrer:E.referrer,referrerPolicy:E.referrerPolicy,signal:E.signal,...A}):N=E;const k=new URL(N,c).href;return D(k),_?$t(N,k,A):Pt(N,A)},setHeaders:()=>{},depends:D,parent(){return v.parent=!0,o()}};Object.defineProperties(R,{props:{get(){throw new Error("@migration task: Replace `props` with `data` stuff https://github.com/sveltejs/kit/discussions/5774#discussioncomment-3292693")},enumerable:!1},session:{get(){throw new Error("session is no longer available. See https://github.com/sveltejs/kit/discussions/5883")},enumerable:!1},stuff:{get(){throw new Error("@migration task: Remove stuff https://github.com/sveltejs/kit/discussions/5774#discussioncomment-3292693")},enumerable:!1},routeId:{get(){throw new Error("routeId has been replaced by route.id")},enumerable:!1}}),m=await b.universal.load.call(null,R)??null,m=m?await Yt(m):null}return{node:b,loader:a,server:d,universal:(O=b.universal)!=null&&O.load?{type:"data",data:m,uses:v}:null,data:m??(d==null?void 0:d.data)??null,slash:((I=b.universal)==null?void 0:I.trailingSlash)??(d==null?void 0:d.slash)}}function Fe(a,o,c,l,p){if(U)return!0;if(!l)return!1;if(l.parent&&a||l.route&&o||l.url&&c)return!0;for(const d of l.params)if(p[d]!==t.params[d])return!0;for(const d of l.dependencies)if(i.some(m=>m(new URL(d))))return!0;return!1}function ve(a,o){return(a==null?void 0:a.type)==="data"?{type:"data",data:a.data,uses:{dependencies:new Set(a.uses.dependencies??[]),params:new Set(a.uses.params??[]),parent:!!a.uses.parent,route:!!a.uses.route,url:!!a.uses.url},slash:a.slash}:(a==null?void 0:a.type)==="skip"?o??null:null}async function Be({id:a,invalidating:o,url:c,params:l,route:p}){if((s==null?void 0:s.id)===a)return s.promise;const{errors:d,layouts:m,leaf:v}=p,b=[...m,v];d.forEach(k=>k==null?void 0:k().catch(()=>{})),b.forEach(k=>k==null?void 0:k[1]().catch(()=>{}));let w=null;const O=t.url?a!==t.url.pathname+t.url.search:!1,I=t.route?a!==t.route.id:!1,D=b.reduce((k,P,j)=>{var x;const L=t.branch[j],H=!!(P!=null&&P[0])&&((L==null?void 0:L.loader)!==P[1]||Fe(k.some(Boolean),I,O,(x=L.server)==null?void 0:x.uses,l));return k.push(H),k},[]);if(D.some(Boolean)){try{w=await nt(c,D)}catch(k){return oe({status:500,error:await ee(k,{url:c,params:l,route:{id:p.id}}),url:c,route:p})}if(w.type==="redirect")return w}const R=w==null?void 0:w.nodes;let E=!1;const A=b.map(async(k,P)=>{var x;if(!k)return;const j=t.branch[P],L=R==null?void 0:R[P];if((!L||L.type==="skip")&&k[1]===(j==null?void 0:j.loader)&&!Fe(E,I,O,(x=j.universal)==null?void 0:x.uses,l))return j;if(E=!0,(L==null?void 0:L.type)==="error")throw L;return be({loader:k[1],url:c,params:l,route:p,parent:async()=>{var He;const ze={};for(let Ee=0;Ee<P;Ee+=1)Object.assign(ze,(He=await A[Ee])==null?void 0:He.data);return ze},server_data_node:ve(L===void 0&&k[0]?{type:"skip"}:L??null,j==null?void 0:j.server)})});for(const k of A)k.catch(()=>{});const N=[];for(let k=0;k<b.length;k+=1)if(b[k])try{N.push(await A[k])}catch(P){if(P instanceof tt)return{type:"redirect",location:P.location};let j=500,L;R!=null&&R.includes(P)?(j=P.status??j,L=P.error):P instanceof $e?(j=P.status,L=P.body):L=await ee(P,{params:l,url:c,route:{id:p.id}});const H=await Je(k,N,d);return H?await Q({url:c,params:l,branch:N.slice(0,H.idx).concat(H.node),status:j,error:L,route:p}):await Ke(c,{id:p.id},L,j)}else N.push(void 0);return await Q({url:c,params:l,branch:N,status:200,error:null,route:p,form:o?void 0:null})}async function Je(a,o,c){for(;a--;)if(c[a]){let l=a;for(;!o[l];)l-=1;try{return{idx:l+1,node:{node:await c[a](),loader:c[a],data:{},server:null,universal:null}}}catch{continue}}}async function oe({status:a,error:o,url:c,route:l}){const p={},d=await Ae();let m=null;if(d.server)try{const w=await nt(c,[!0]);if(w.type!=="data"||w.nodes[0]&&w.nodes[0].type!=="data")throw 0;m=w.nodes[0]??null}catch{(c.origin!==location.origin||c.pathname!==location.pathname||f)&&await le(c)}const v=await be({loader:Ae,url:c,params:p,route:l,parent:()=>Promise.resolve({}),server_data_node:ve(m)}),b={node:await je(),loader:je,universal:null,server:null,data:null};return await Q({url:c,params:p,branch:[v,b],status:a,error:o,route:null})}function se(a,o){if(Xe(a,e))return;const c=vt(a.pathname.slice(e.length)||"/");for(const l of Oe){const p=l.exec(c);if(p)return{id:a.pathname+a.search,invalidating:o,route:l,params:Et(p),url:a}}}function Ge({url:a,type:o,intent:c,delta:l}){var v,b;let p=!1;const d={from:ue("from",{params:t.params,route:{id:((v=t.route)==null?void 0:v.id)??null},url:t.url}),to:ue("to",{params:(c==null?void 0:c.params)??null,route:{id:((b=c==null?void 0:c.route)==null?void 0:b.id)??null},url:a}),willUnload:!c,type:o};l!==void 0&&(d.delta=l);const m={...d,cancel:()=>{p=!0}};return y||u.before_navigate.forEach(w=>w(m)),p?null:d}async function ie({url:a,scroll:o,keepfocus:c,redirect_chain:l,details:p,type:d,delta:m,nav_token:v,accepted:b,blocked:w}){const O=se(a,!1),I=Ge({url:a,type:d,delta:m,intent:O});if(!I){w();return}Ie($),b(),y=!0,_&&M.navigating.set(I),await Ce(O,a,l,{scroll:o,keepfocus:c,details:p},v,()=>{y=!1,u.after_navigate.forEach(D=>D(I)),M.navigating.set(null)})}async function Ke(a,o,c,l){return a.origin===location.origin&&a.pathname===location.pathname&&!f?await oe({status:l,error:c,url:a,route:o}):await le(a)}function le(a){return location.href=a.href,new Promise(()=>{})}function at(){let a;n.addEventListener("mousemove",d=>{const m=d.target;clearTimeout(a),a=setTimeout(()=>{l(m,2)},20)});function o(d){l(d.composedPath()[0],1)}n.addEventListener("mousedown",o),n.addEventListener("touchstart",o,{passive:!0});const c=new IntersectionObserver(d=>{for(const m of d)m.isIntersecting&&(ae(new URL(m.target.href).pathname),c.unobserve(m.target))},{threshold:0});function l(d,m){const v=Ye(d,n);if(!v)return;const{url:b,external:w}=Se(v,e);if(w)return;const O=fe(v);O.reload||(m<=O.preload_data?Ve(b):m<=O.preload_code&&ae(b.pathname))}function p(){c.disconnect();for(const d of n.querySelectorAll("a")){const{url:m,external:v}=Se(d,e);if(v)continue;const b=fe(d);b.reload||(b.preload_code===Ze.viewport&&c.observe(d),b.preload_code===Ze.eager&&ae(m.pathname))}}u.after_navigate.push(p),p()}return{after_navigate:a=>{Pe(()=>(u.after_navigate.push(a),()=>{const o=u.after_navigate.indexOf(a);u.after_navigate.splice(o,1)}))},before_navigate:a=>{Pe(()=>(u.before_navigate.push(a),()=>{const o=u.before_navigate.indexOf(a);u.before_navigate.splice(o,1)}))},disable_scroll_handling:()=>{(g||!_)&&(h=!1)},goto:(a,o={})=>{if("keepfocus"in o&&!("keepFocus"in o))throw new Error("`keepfocus` has been renamed to `keepFocus` (note the difference in casing)");if("noscroll"in o&&!("noScroll"in o))throw new Error("`noscroll` has been renamed to `noScroll` (note the difference in casing)");return ye(a,o,[])},invalidate:a=>{if(a===void 0)throw new Error("`invalidate()` (with no arguments) has been replaced by `invalidateAll()`");if(typeof a=="function")i.push(a);else{const{href:o}=new URL(a,location.href);i.push(c=>c.href===o)}return De()},invalidateAll:()=>(U=!0,De()),preload_data:async a=>{const o=new URL(a,We(document));await Ve(o)},preload_code:ae,apply_action:async a=>{if(a.type==="error"){const o=new URL(location.href),{branch:c,route:l}=t;if(!l)return;const p=await Je(t.branch.length,c,l.errors);if(p){const d=await Q({url:o,params:t.params,branch:c.slice(0,p.idx).concat(p.node),status:a.status??500,error:a.error,route:l});t=d.state;const m=de();T.$set(d.props),m(),ce().then(Le)}}else if(a.type==="redirect")ye(a.location,{invalidateAll:!0},[]);else{const o={form:a.data,page:{...K,form:a.data,status:a.status}},c=de();T.$set(o),c(),a.type==="success"&&ce().then(Le)}},_start_router:()=>{var a;history.scrollRestoration="manual",addEventListener("beforeunload",o=>{var l;let c=!1;if(!y){const p={from:ue("from",{params:t.params,route:{id:((l=t.route)==null?void 0:l.id)??null},url:t.url}),to:null,willUnload:!0,type:"leave",cancel:()=>c=!0};u.before_navigate.forEach(d=>d(p))}c?(o.preventDefault(),o.returnValue=""):history.scrollRestoration="auto"}),addEventListener("visibilitychange",()=>{if(document.visibilityState==="hidden"){Ie($);try{sessionStorage[rt]=JSON.stringify(ne)}catch{}}}),(a=navigator.connection)!=null&&a.saveData||at(),n.addEventListener("click",o=>{if(o.button||o.which!==1||o.metaKey||o.ctrlKey||o.shiftKey||o.altKey||o.defaultPrevented)return;const c=Ye(o.composedPath()[0],n);if(!c)return;const{url:l,external:p,has:d}=Se(c,e),m=fe(c);if(!l||!(c instanceof SVGAElement)&&l.protocol!==location.protocol&&!(l.protocol==="https:"||l.protocol==="http:")||d.download)return;if(p||m.reload){Ge({url:l,type:"link"})||o.preventDefault(),y=!0;return}const[b,w]=l.href.split("#");if(w!==void 0&&b===location.href.split("#")[0]){S=!0,Ie($),t.url=l,M.page.set({...K,url:l}),M.page.notify();return}ie({url:l,scroll:m.noscroll?pe():null,keepfocus:!1,redirect_chain:[],details:{state:{},replaceState:l.href===location.href},accepted:()=>o.preventDefault(),blocked:()=>o.preventDefault(),type:"link"})}),n.addEventListener("submit",o=>{var b;if(o.defaultPrevented)return;const c=HTMLFormElement.prototype.cloneNode.call(o.target),l=o.submitter;if(((l==null?void 0:l.formMethod)||c.method)!=="get")return;const d=new URL(((b=o.submitter)==null?void 0:b.hasAttribute("formaction"))&&(l==null?void 0:l.formAction)||c.action);if(Xe(d,e))return;const{noscroll:m,reload:v}=fe(o.target);v||(o.preventDefault(),o.stopPropagation(),d.search=new URLSearchParams(new FormData(o.target)).toString(),ie({url:d,scroll:m?pe():null,keepfocus:!1,redirect_chain:[],details:{state:{},replaceState:!1},nav_token:{},accepted:()=>{},blocked:()=>{},type:"form"}))}),addEventListener("popstate",o=>{var c;if((c=o.state)!=null&&c[C]){if(o.state[C]===$)return;const l=o.state[C]-$;ie({url:new URL(location.href),scroll:ne[o.state[C]],keepfocus:!1,redirect_chain:[],details:null,accepted:()=>{$=o.state[C]},blocked:()=>{history.go(-l)},type:"popstate",delta:l})}}),addEventListener("hashchange",()=>{S&&(S=!1,history.replaceState({...history.state,[C]:++$},"",location.href))});for(const o of document.querySelectorAll("link"))o.rel==="icon"&&(o.href=o.href);addEventListener("pageshow",o=>{o.persisted&&M.navigating.set(null)})},_hydrate:async({status:a=200,error:o,node_ids:c,params:l,route:p,data:d,form:m})=>{f=!0;const v=new URL(location.href);({params:l={},route:p={id:null}}=se(v,!1)||{});let b;try{const w=c.map(async(O,I)=>{const D=d[I];return be({loader:ge[O],url:v,params:l,route:p,parent:async()=>{const R={};for(let E=0;E<I;E+=1)Object.assign(R,(await w[E]).data);return R},server_data_node:ve(D)})});b=await Q({url:v,params:l,branch:await Promise.all(w),status:a,error:o,form:m,route:Oe.find(({id:O})=>O===p.id)??null})}catch(w){if(w instanceof tt){await le(new URL(w.location,location.href));return}b=await oe({status:w instanceof $e?w.status:500,error:await ee(w,{url:v,params:l,route:p}),url:v,route:p})}qe(b)}}}async function nt(r,e){var u;const n=new URL(r);n.pathname=It(r.pathname),n.searchParams.append("x-sveltekit-invalidated",e.map(t=>t?"1":"").join("_"));const i=await _e(n.href),s=await i.json();if(!i.ok)throw new Error(s);return(u=s.nodes)==null||u.forEach(t=>{(t==null?void 0:t.type)==="data"&&(t.data=nn(t.data),t.uses={dependencies:new Set(t.uses.dependencies??[]),params:new Set(t.uses.params??[]),parent:!!t.uses.parent,route:!!t.uses.route,url:!!t.uses.url})}),s}function ee(r,e){return r instanceof $e?r.body:Wt.handleError({error:r,event:e})??{message:e.route.id!=null?"Internal Error":"Not Found"}}const an=["hash","href","host","hostname","origin","pathname","port","protocol","search","searchParams","toString","toJSON"];function ue(r,e){for(const n of an)Object.defineProperty(e,n,{get(){throw new Error(`The navigation shape changed - ${r}.${n} should now be ${r}.url.${n}`)},enumerable:!1});return Object.defineProperty(e,"routeId",{get(){throw new Error(`The navigation shape changed - ${r}.routeId should now be ${r}.route.id`)},enumerable:!1}),e}function de(){return()=>{}}function Le(){const r=document.querySelector("[autofocus]");if(r)r.focus();else{const e=document.body,n=e.getAttribute("tabindex");e.tabIndex=-1,e.focus({preventScroll:!0}),setTimeout(()=>{var i;(i=getSelection())==null||i.removeAllRanges()}),n!==null?e.setAttribute("tabindex",n):e.removeAttribute("tabindex")}}async function ln({env:r,hydrate:e,paths:n,target:i,version:s}){gt(n),yt(s);const u=rn({target:i,base:n.base});wt({client:u}),e?await u._hydrate(e):u.goto(location.href,{replaceState:!0}),u._start_router()}export{ln as start};
