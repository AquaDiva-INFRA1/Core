var app=function(){"use strict";function t(){}function n(t,n){for(const e in n)t[e]=n[e];return t}function e(t){return t()}function o(){return Object.create(null)}function l(t){t.forEach(e)}function c(t){return"function"==typeof t}function r(t,n){return t!=t?n==n:t!==n||t&&"object"==typeof t||"function"==typeof t}function s(t,n,e,o){if(t){const l=i(t,n,e,o);return t[0](l)}}function i(t,e,o,l){return t[1]&&l?n(o.ctx.slice(),t[1](l(e))):o.ctx}function u(t,n,e,o){if(t[2]&&o){const l=t[2](o(e));if(void 0===n.dirty)return l;if("object"==typeof l){const t=[],e=Math.max(n.dirty.length,l.length);for(let o=0;o<e;o+=1)t[o]=n.dirty[o]|l[o];return t}return n.dirty|l}return n.dirty}function f(t,n,e,o,l,c){if(l){const r=i(n,e,o,c);t.p(r,l)}}function a(t){if(t.ctx.length>32){const n=[],e=t.ctx.length/32;for(let t=0;t<e;t++)n[t]=-1;return n}return-1}function d(t){const n={};for(const e in t)"$"!==e[0]&&(n[e]=t[e]);return n}function p(t,n){const e={};n=new Set(n);for(const o in t)n.has(o)||"$"===o[0]||(e[o]=t[o]);return e}function $(t,n){t.appendChild(n)}function m(t,n,e){t.insertBefore(n,e||null)}function h(t){t.parentNode.removeChild(t)}function g(t,n){for(let e=0;e<t.length;e+=1)t[e]&&t[e].d(n)}function b(t){return document.createElement(t)}function y(t){return document.createTextNode(t)}function x(){return y(" ")}function v(){return y("")}function _(t,n,e,o){return t.addEventListener(n,e,o),()=>t.removeEventListener(n,e,o)}function w(t,n,e){null==e?t.removeAttribute(n):t.getAttribute(n)!==e&&t.setAttribute(n,e)}function k(t,n){const e=Object.getOwnPropertyDescriptors(t.__proto__);for(const o in n)null==n[o]?t.removeAttribute(o):"style"===o?t.style.cssText=n[o]:"__value"===o?t.value=t[o]=n[o]:e[o]&&e[o].set?t[o]=n[o]:w(t,o,n[o])}function E(t,n){n=""+n,t.wholeText!==n&&(t.data=n)}let j;function N(t){j=t}function z(t){(function(){if(!j)throw new Error("Function called outside component initialization");return j})().$$.on_mount.push(t)}function A(t,n){const e=t.$$.callbacks[n.type];e&&e.slice().forEach((t=>t.call(this,n)))}const B=[],O=[],L=[],S=[],T=Promise.resolve();let C=!1;function M(t){L.push(t)}let P=!1;const q=new Set;function D(){if(!P){P=!0;do{for(let t=0;t<B.length;t+=1){const n=B[t];N(n),F(n.$$)}for(N(null),B.length=0;O.length;)O.pop()();for(let t=0;t<L.length;t+=1){const n=L[t];q.has(n)||(q.add(n),n())}L.length=0}while(B.length);for(;S.length;)S.pop()();C=!1,P=!1,q.clear()}}function F(t){if(null!==t.fragment){t.update(),l(t.before_update);const n=t.dirty;t.dirty=[-1],t.fragment&&t.fragment.p(t.ctx,n),t.after_update.forEach(M)}}const I=new Set;let G;function H(){G={r:0,c:[],p:G}}function J(){G.r||l(G.c),G=G.p}function K(t,n){t&&t.i&&(I.delete(t),t.i(n))}function Q(t,n,e,o){if(t&&t.o){if(I.has(t))return;I.add(t),G.c.push((()=>{I.delete(t),o&&(e&&t.d(1),o())})),t.o(n)}}function R(t,n){const e={},o={},l={$$scope:1};let c=t.length;for(;c--;){const r=t[c],s=n[c];if(s){for(const t in r)t in s||(o[t]=1);for(const t in s)l[t]||(e[t]=s[t],l[t]=1);t[c]=s}else for(const t in r)l[t]=1}for(const t in o)t in e||(e[t]=void 0);return e}function U(t){t&&t.c()}function V(t,n,o,r){const{fragment:s,on_mount:i,on_destroy:u,after_update:f}=t.$$;s&&s.m(n,o),r||M((()=>{const n=i.map(e).filter(c);u?u.push(...n):l(n),t.$$.on_mount=[]})),f.forEach(M)}function W(t,n){const e=t.$$;null!==e.fragment&&(l(e.on_destroy),e.fragment&&e.fragment.d(n),e.on_destroy=e.fragment=null,e.ctx=[])}function X(t,n){-1===t.$$.dirty[0]&&(B.push(t),C||(C=!0,T.then(D)),t.$$.dirty.fill(0)),t.$$.dirty[n/31|0]|=1<<n%31}function Y(n,e,c,r,s,i,u,f=[-1]){const a=j;N(n);const d=n.$$={fragment:null,ctx:null,props:i,update:t,not_equal:s,bound:o(),on_mount:[],on_destroy:[],on_disconnect:[],before_update:[],after_update:[],context:new Map(e.context||(a?a.$$.context:[])),callbacks:o(),dirty:f,skip_bound:!1,root:e.target||a.$$.root};u&&u(d.root);let p=!1;if(d.ctx=c?c(n,e.props||{},((t,e,...o)=>{const l=o.length?o[0]:e;return d.ctx&&s(d.ctx[t],d.ctx[t]=l)&&(!d.skip_bound&&d.bound[t]&&d.bound[t](l),p&&X(n,t)),e})):[],d.update(),p=!0,l(d.before_update),d.fragment=!!r&&r(d.ctx),e.target){if(e.hydrate){const t=function(t){return Array.from(t.childNodes)}(e.target);d.fragment&&d.fragment.l(t),t.forEach(h)}else d.fragment&&d.fragment.c();e.intro&&K(n.$$.fragment),V(n,e.target,e.anchor,e.customElement),D()}N(a)}class Z{$destroy(){W(this,1),this.$destroy=t}$on(t,n){const e=this.$$.callbacks[t]||(this.$$.callbacks[t]=[]);return e.push(n),()=>{const t=e.indexOf(n);-1!==t&&e.splice(t,1)}}$set(t){var n;this.$$set&&(n=t,0!==Object.keys(n).length)&&(this.$$.skip_bound=!0,this.$$set(t),this.$$.skip_bound=!1)}}function tt(t){let n="";if("string"==typeof t||"number"==typeof t)n+=t;else if("object"==typeof t)if(Array.isArray(t))n=t.map(tt).filter(Boolean).join(" ");else for(let e in t)t[e]&&(n&&(n+=" "),n+=e);return n}function nt(...t){return t.map(tt).filter(Boolean).join(" ")}function et(t){let e,o;const l=t[6].default,c=s(l,t,t[5],null);let r=[t[2],{class:t[1]}],i={};for(let t=0;t<r.length;t+=1)i=n(i,r[t]);return{c(){e=b("ul"),c&&c.c(),k(e,i)},m(t,n){m(t,e,n),c&&c.m(e,null),o=!0},p(t,n){c&&c.p&&(!o||32&n)&&f(c,l,t,t[5],o?u(l,t[5],n,null):a(t[5]),null),k(e,i=R(r,[4&n&&t[2],(!o||2&n)&&{class:t[1]}]))},i(t){o||(K(c,t),o=!0)},o(t){Q(c,t),o=!1},d(t){t&&h(e),c&&c.d(t)}}}function ot(t){let e,o;const l=t[6].default,c=s(l,t,t[5],null);let r=[t[2],{class:t[1]}],i={};for(let t=0;t<r.length;t+=1)i=n(i,r[t]);return{c(){e=b("ol"),c&&c.c(),k(e,i)},m(t,n){m(t,e,n),c&&c.m(e,null),o=!0},p(t,n){c&&c.p&&(!o||32&n)&&f(c,l,t,t[5],o?u(l,t[5],n,null):a(t[5]),null),k(e,i=R(r,[4&n&&t[2],(!o||2&n)&&{class:t[1]}]))},i(t){o||(K(c,t),o=!0)},o(t){Q(c,t),o=!1},d(t){t&&h(e),c&&c.d(t)}}}function lt(t){let n,e,o,l;const c=[ot,et],r=[];function s(t,n){return t[0]?0:1}return n=s(t),e=r[n]=c[n](t),{c(){e.c(),o=v()},m(t,e){r[n].m(t,e),m(t,o,e),l=!0},p(t,[l]){let i=n;n=s(t),n===i?r[n].p(t,l):(H(),Q(r[i],1,1,(()=>{r[i]=null})),J(),e=r[n],e?e.p(t,l):(e=r[n]=c[n](t),e.c()),K(e,1),e.m(o.parentNode,o))},i(t){l||(K(e),l=!0)},o(t){Q(e),l=!1},d(t){r[n].d(t),t&&h(o)}}}function ct(t,e,o){let l;const c=["class","flush","numbered"];let r=p(e,c),{$$slots:s={},$$scope:i}=e,{class:u=""}=e,{flush:f=!1}=e,{numbered:a=!1}=e;return t.$$set=t=>{e=n(n({},e),d(t)),o(2,r=p(e,c)),"class"in t&&o(3,u=t.class),"flush"in t&&o(4,f=t.flush),"numbered"in t&&o(0,a=t.numbered),"$$scope"in t&&o(5,i=t.$$scope)},t.$$.update=()=>{25&t.$$.dirty&&o(1,l=nt(u,"list-group",{"list-group-flush":f,"list-group-numbered":a}))},[a,l,r,u,f,i,s]}class rt extends Z{constructor(t){super(),Y(this,t,ct,lt,r,{class:3,flush:4,numbered:0})}}function st(t){let e,o,l,c;const r=t[10].default,i=s(r,t,t[9],null);let d=[t[5],{class:t[4]},{disabled:t[1]},{active:t[0]}],p={};for(let t=0;t<d.length;t+=1)p=n(p,d[t]);return{c(){e=b("li"),i&&i.c(),k(e,p)},m(n,r){m(n,e,r),i&&i.m(e,null),o=!0,l||(c=_(e,"click",t[13]),l=!0)},p(t,n){i&&i.p&&(!o||512&n)&&f(i,r,t,t[9],o?u(r,t[9],n,null):a(t[9]),null),k(e,p=R(d,[32&n&&t[5],(!o||16&n)&&{class:t[4]},(!o||2&n)&&{disabled:t[1]},(!o||1&n)&&{active:t[0]}]))},i(t){o||(K(i,t),o=!0)},o(t){Q(i,t),o=!1},d(t){t&&h(e),i&&i.d(t),l=!1,c()}}}function it(t){let e,o,l,c;const r=t[10].default,i=s(r,t,t[9],null);let d=[t[5],{class:t[4]},{type:"button"},{disabled:t[1]},{active:t[0]}],p={};for(let t=0;t<d.length;t+=1)p=n(p,d[t]);return{c(){e=b("button"),i&&i.c(),k(e,p)},m(n,r){m(n,e,r),i&&i.m(e,null),e.autofocus&&e.focus(),o=!0,l||(c=_(e,"click",t[12]),l=!0)},p(t,n){i&&i.p&&(!o||512&n)&&f(i,r,t,t[9],o?u(r,t[9],n,null):a(t[9]),null),k(e,p=R(d,[32&n&&t[5],(!o||16&n)&&{class:t[4]},{type:"button"},(!o||2&n)&&{disabled:t[1]},(!o||1&n)&&{active:t[0]}]))},i(t){o||(K(i,t),o=!0)},o(t){Q(i,t),o=!1},d(t){t&&h(e),i&&i.d(t),l=!1,c()}}}function ut(t){let e,o,l,c;const r=t[10].default,i=s(r,t,t[9],null);let d=[t[5],{class:t[4]},{href:t[2]},{disabled:t[1]},{active:t[0]}],p={};for(let t=0;t<d.length;t+=1)p=n(p,d[t]);return{c(){e=b("a"),i&&i.c(),k(e,p)},m(n,r){m(n,e,r),i&&i.m(e,null),o=!0,l||(c=_(e,"click",t[11]),l=!0)},p(t,n){i&&i.p&&(!o||512&n)&&f(i,r,t,t[9],o?u(r,t[9],n,null):a(t[9]),null),k(e,p=R(d,[32&n&&t[5],(!o||16&n)&&{class:t[4]},(!o||4&n)&&{href:t[2]},(!o||2&n)&&{disabled:t[1]},(!o||1&n)&&{active:t[0]}]))},i(t){o||(K(i,t),o=!0)},o(t){Q(i,t),o=!1},d(t){t&&h(e),i&&i.d(t),l=!1,c()}}}function ft(t){let n,e,o,l;const c=[ut,it,st],r=[];function s(t,n){return t[2]?0:"button"===t[3]?1:2}return n=s(t),e=r[n]=c[n](t),{c(){e.c(),o=v()},m(t,e){r[n].m(t,e),m(t,o,e),l=!0},p(t,[l]){let i=n;n=s(t),n===i?r[n].p(t,l):(H(),Q(r[i],1,1,(()=>{r[i]=null})),J(),e=r[n],e?e.p(t,l):(e=r[n]=c[n](t),e.c()),K(e,1),e.m(o.parentNode,o))},i(t){l||(K(e),l=!0)},o(t){Q(e),l=!1},d(t){r[n].d(t),t&&h(o)}}}function at(t,e,o){let l;const c=["class","active","disabled","color","action","href","tag"];let r=p(e,c),{$$slots:s={},$$scope:i}=e,{class:u=""}=e,{active:f=!1}=e,{disabled:a=!1}=e,{color:$=""}=e,{action:m=!1}=e,{href:h=null}=e,{tag:g=null}=e;return t.$$set=t=>{e=n(n({},e),d(t)),o(5,r=p(e,c)),"class"in t&&o(6,u=t.class),"active"in t&&o(0,f=t.active),"disabled"in t&&o(1,a=t.disabled),"color"in t&&o(7,$=t.color),"action"in t&&o(8,m=t.action),"href"in t&&o(2,h=t.href),"tag"in t&&o(3,g=t.tag),"$$scope"in t&&o(9,i=t.$$scope)},t.$$.update=()=>{459&t.$$.dirty&&o(4,l=nt(u,"list-group-item",{active:f,disabled:a,"list-group-item-action":m||"button"===g,[`list-group-item-${$}`]:$}))},[f,a,h,g,l,r,u,$,m,i,s,function(n){A.call(this,t,n)},function(n){A.call(this,t,n)},function(n){A.call(this,t,n)}]}class dt extends Z{constructor(t){super(),Y(this,t,at,ft,r,{class:6,active:0,disabled:1,color:7,action:8,href:2,tag:3})}}function pt(t){let e,o,l;const c=t[7].default,r=s(c,t,t[6],null),i=r||function(t){let n;return{c(){n=y("Loading...")},m(t,e){m(t,n,e)},d(t){t&&h(n)}}}();let d=[t[1],{role:"status"},{class:t[0]}],p={};for(let t=0;t<d.length;t+=1)p=n(p,d[t]);return{c(){e=b("div"),o=b("span"),i&&i.c(),w(o,"class","visually-hidden"),k(e,p)},m(t,n){m(t,e,n),$(e,o),i&&i.m(o,null),l=!0},p(t,[n]){r&&r.p&&(!l||64&n)&&f(r,c,t,t[6],l?u(c,t[6],n,null):a(t[6]),null),k(e,p=R(d,[2&n&&t[1],{role:"status"},(!l||1&n)&&{class:t[0]}]))},i(t){l||(K(i,t),l=!0)},o(t){Q(i,t),l=!1},d(t){t&&h(e),i&&i.d(t)}}}function $t(t,e,o){let l;const c=["class","type","size","color"];let r=p(e,c),{$$slots:s={},$$scope:i}=e,{class:u=""}=e,{type:f="border"}=e,{size:a=""}=e,{color:$=""}=e;return t.$$set=t=>{e=n(n({},e),d(t)),o(1,r=p(e,c)),"class"in t&&o(2,u=t.class),"type"in t&&o(3,f=t.type),"size"in t&&o(4,a=t.size),"color"in t&&o(5,$=t.color),"$$scope"in t&&o(6,i=t.$$scope)},t.$$.update=()=>{60&t.$$.dirty&&o(0,l=nt(u,!!a&&`spinner-${f}-${a}`,`spinner-${f}`,!!$&&`text-${$}`))},[l,r,u,f,a,$,i,s]}class mt extends Z{constructor(t){super(),Y(this,t,$t,pt,r,{class:2,type:3,size:4,color:5})}}function ht(t,n,e){const o=t.slice();return o[1]=n[e],o}function gt(t,n,e){const o=t.slice();return o[4]=n[e],o}function bt(n){let e,o;return e=new mt({props:{color:"primary",size:"sm",type:"grow","text-center":!0}}),{c(){U(e.$$.fragment)},m(t,n){V(e,t,n),o=!0},p:t,i(t){o||(K(e.$$.fragment,t),o=!0)},o(t){Q(e.$$.fragment,t),o=!1},d(t){W(e,t)}}}function yt(t){let n,e;return n=new rt({props:{flush:!0,$$slots:{default:[wt]},$$scope:{ctx:t}}}),{c(){U(n.$$.fragment)},m(t,o){V(n,t,o),e=!0},p(t,e){const o={};129&e&&(o.$$scope={dirty:e,ctx:t}),n.$set(o)},i(t){e||(K(n.$$.fragment,t),e=!0)},o(t){Q(n.$$.fragment,t),e=!1},d(t){W(n,t)}}}function xt(t){let n,e=t[4]+"";return{c(){n=y(e)},m(t,e){m(t,n,e)},p(t,o){1&o&&e!==(e=t[4]+"")&&E(n,e)},d(t){t&&h(n)}}}function vt(t){let n,e,o,l,c,r,s=t[1].timestamp+"",i=t[1].messages,u=[];for(let n=0;n<i.length;n+=1)u[n]=xt(gt(t,i,n));return{c(){n=b("b"),e=y(s),o=x(),l=b("br"),c=x();for(let t=0;t<u.length;t+=1)u[t].c();r=x()},m(t,s){m(t,n,s),$(n,e),m(t,o,s),m(t,l,s),m(t,c,s);for(let n=0;n<u.length;n+=1)u[n].m(t,s);m(t,r,s)},p(t,n){if(1&n&&s!==(s=t[1].timestamp+"")&&E(e,s),1&n){let e;for(i=t[1].messages,e=0;e<i.length;e+=1){const o=gt(t,i,e);u[e]?u[e].p(o,n):(u[e]=xt(o),u[e].c(),u[e].m(r.parentNode,r))}for(;e<u.length;e+=1)u[e].d(1);u.length=i.length}},d(t){t&&h(n),t&&h(o),t&&h(l),t&&h(c),g(u,t),t&&h(r)}}}function _t(t){let n,e;return n=new dt({props:{$$slots:{default:[vt]},$$scope:{ctx:t}}}),{c(){U(n.$$.fragment)},m(t,o){V(n,t,o),e=!0},p(t,e){const o={};129&e&&(o.$$scope={dirty:e,ctx:t}),n.$set(o)},i(t){e||(K(n.$$.fragment,t),e=!0)},o(t){Q(n.$$.fragment,t),e=!1},d(t){W(n,t)}}}function wt(t){let n,e,o=t[0],l=[];for(let n=0;n<o.length;n+=1)l[n]=_t(ht(t,o,n));const c=t=>Q(l[t],1,1,(()=>{l[t]=null}));return{c(){for(let t=0;t<l.length;t+=1)l[t].c();n=v()},m(t,o){for(let n=0;n<l.length;n+=1)l[n].m(t,o);m(t,n,o),e=!0},p(t,e){if(1&e){let r;for(o=t[0],r=0;r<o.length;r+=1){const c=ht(t,o,r);l[r]?(l[r].p(c,e),K(l[r],1)):(l[r]=_t(c),l[r].c(),K(l[r],1),l[r].m(n.parentNode,n))}for(H(),r=o.length;r<l.length;r+=1)c(r);J()}},i(t){if(!e){for(let t=0;t<o.length;t+=1)K(l[t]);e=!0}},o(t){l=l.filter(Boolean);for(let t=0;t<l.length;t+=1)Q(l[t]);e=!1},d(t){g(l,t),t&&h(n)}}}function kt(t){let n,e,o,l;const c=[yt,bt],r=[];function s(t,n){return t[0]?0:1}return n=s(t),e=r[n]=c[n](t),{c(){e.c(),o=v()},m(t,e){r[n].m(t,e),m(t,o,e),l=!0},p(t,[l]){let i=n;n=s(t),n===i?r[n].p(t,l):(H(),Q(r[i],1,1,(()=>{r[i]=null})),J(),e=r[n],e?e.p(t,l):(e=r[n]=c[n](t),e.c()),K(e,1),e.m(o.parentNode,o))},i(t){l||(K(e),l=!0)},o(t){Q(e),l=!1},d(t){r[n].d(t),t&&h(o)}}}function Et(t,n,e){let o;return console.log("in resultmessages"),z((async()=>{console.log("call on mount");const t=await fetch("https://localhost:44345/dcm/messages/load?id=1");e(0,o=await t.json()),console.log(o)})),e(0,o=null),[o]}return new class extends Z{constructor(t){super(),Y(this,t,Et,kt,r,{})}}({target:document.getElementById("messages")})}();
//# sourceMappingURL=messages.js.map
