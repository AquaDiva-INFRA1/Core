import{S as le,i as ne,s as ie,C as ce,k as A,a as G,l as L,m as T,h as b,c as K,n as y,b as I,G as V,K as R,D as de,E as _e,F as be,g as v,v as W,d as p,f as J,L as je,ac as qe,aa as $,R as te,ab as ue,M as z,a2 as Qe,ai as ge,T as Ce,al as ve,ad as ee,w as He,q as P,e as q,r as S,H as X,P as me,u as F,o as Re,y as Y,z as Z,A as w,B as x,V as Xe,W as Ye,U as Ze}from"../chunks/index.5cde3559.js";import{A as we,s as xe}from"../chunks/store.53a40dd4.js";import{S as Ue}from"../chunks/Spinner.2e89e3a9.js";const $e=a=>({}),ke=a=>({});function ye(a){let e,n,l;const t=a[17].panel,s=ce(t,a,a[16],ke);return{c(){e=A("div"),s&&s.c(),this.h()},l(i){e=L(i,"DIV",{class:!0,role:!0,"aria-labelledby":!0,tabindex:!0});var r=T(e);s&&s.l(r),r.forEach(b),this.h()},h(){y(e,"class",n="tab-panel "+a[2]),y(e,"role","tabpanel"),y(e,"aria-labelledby",a[1]),y(e,"tabindex","0")},m(i,r){I(i,e,r),s&&s.m(e,null),l=!0},p(i,r){s&&s.p&&(!l||r&65536)&&de(s,t,i,i[16],l?be(t,i[16],r,$e):_e(i[16]),ke),(!l||r&4&&n!==(n="tab-panel "+i[2]))&&y(e,"class",n),(!l||r&2)&&y(e,"aria-labelledby",i[1])},i(i){l||(v(s,i),l=!0)},o(i){p(s,i),l=!1},d(i){i&&b(e),s&&s.d(i)}}}function et(a){let e,n,l,t,s,i,r,o;const u=a[17].default,f=ce(u,a,a[16],null);let c=a[5].panel&&ye(a);return{c(){e=A("div"),n=A("div"),f&&f.c(),t=G(),c&&c.c(),this.h()},l(_){e=L(_,"DIV",{class:!0,"data-testid":!0});var d=T(e);n=L(d,"DIV",{class:!0,role:!0,"aria-labelledby":!0});var k=T(n);f&&f.l(k),k.forEach(b),t=K(d),c&&c.l(d),d.forEach(b),this.h()},h(){y(n,"class",l="tab-list "+a[3]),y(n,"role","tablist"),y(n,"aria-labelledby",a[0]),y(e,"class",s="tab-group "+a[4]),y(e,"data-testid","tab-group")},m(_,d){I(_,e,d),V(e,n),f&&f.m(n,null),V(e,t),c&&c.m(e,null),i=!0,r||(o=[R(e,"click",a[18]),R(e,"keypress",a[19]),R(e,"keydown",a[20]),R(e,"keyup",a[21])],r=!0)},p(_,[d]){f&&f.p&&(!i||d&65536)&&de(f,u,_,_[16],i?be(u,_[16],d,null):_e(_[16]),null),(!i||d&8&&l!==(l="tab-list "+_[3]))&&y(n,"class",l),(!i||d&1)&&y(n,"aria-labelledby",_[0]),_[5].panel?c?(c.p(_,d),d&32&&v(c,1)):(c=ye(_),c.c(),v(c,1),c.m(e,null)):c&&(W(),p(c,1,1,()=>{c=null}),J()),(!i||d&16&&s!==(s="tab-group "+_[4]))&&y(e,"class",s)},i(_){i||(v(f,_),v(c),i=!0)},o(_){p(f,_),p(c),i=!1},d(_){_&&b(e),f&&f.d(_),c&&c.d(),r=!1,je(o)}}}const tt="space-y-4",lt="flex overflow-x-auto hide-scrollbar",nt="";function it(a,e,n){let l,t,s,{$$slots:i={},$$scope:r}=e;const o=qe(i);let{justify:u="justify-start"}=e,{border:f="border-b border-surface-400-500-token"}=e,{active:c="border-b-2 border-surface-900-50-token"}=e,{hover:_="hover:variant-soft"}=e,{flex:d="flex-none"}=e,{padding:k="px-4 py-2"}=e,{rounded:B="rounded-tl-container-token rounded-tr-container-token"}=e,{spacing:j="space-y-1"}=e,{regionList:H=""}=e,{regionPanel:E=""}=e,{labelledby:M=""}=e,{panel:N=""}=e;$("active",c),$("hover",_),$("flex",d),$("padding",k),$("rounded",B),$("spacing",j);function g(m){z.call(this,a,m)}function D(m){z.call(this,a,m)}function O(m){z.call(this,a,m)}function C(m){z.call(this,a,m)}return a.$$set=m=>{n(22,e=te(te({},e),ue(m))),"justify"in m&&n(6,u=m.justify),"border"in m&&n(7,f=m.border),"active"in m&&n(8,c=m.active),"hover"in m&&n(9,_=m.hover),"flex"in m&&n(10,d=m.flex),"padding"in m&&n(11,k=m.padding),"rounded"in m&&n(12,B=m.rounded),"spacing"in m&&n(13,j=m.spacing),"regionList"in m&&n(14,H=m.regionList),"regionPanel"in m&&n(15,E=m.regionPanel),"labelledby"in m&&n(0,M=m.labelledby),"panel"in m&&n(1,N=m.panel),"$$scope"in m&&n(16,r=m.$$scope)},a.$$.update=()=>{n(4,l=`${tt} ${e.class??""}`),a.$$.dirty&16576&&n(3,t=`${lt} ${u} ${f} ${H}`),a.$$.dirty&32768&&n(2,s=`${nt} ${E}`)},e=ue(e),[M,N,s,t,l,o,u,f,c,_,d,k,B,j,H,E,r,i,g,D,O,C]}class at extends le{constructor(e){super(),ne(this,e,it,et,ie,{justify:6,border:7,active:8,hover:9,flex:10,padding:11,rounded:12,spacing:13,regionList:14,regionPanel:15,labelledby:0,panel:1})}}const st=a=>({}),Ie=a=>({});function pe(a){let e,n;const l=a[21].lead,t=ce(l,a,a[20],Ie);return{c(){e=A("div"),t&&t.c(),this.h()},l(s){e=L(s,"DIV",{class:!0});var i=T(e);t&&t.l(i),i.forEach(b),this.h()},h(){y(e,"class","tab-lead")},m(s,i){I(s,e,i),t&&t.m(e,null),n=!0},p(s,i){t&&t.p&&(!n||i[0]&1048576)&&de(t,l,s,s[20],n?be(l,s[20],i,st):_e(s[20]),Ie)},i(s){n||(v(t,s),n=!0)},o(s){p(t,s),n=!1},d(s){s&&b(e),t&&t.d(s)}}}function ot(a){let e,n,l,t,s,i,r,o,u,f,c,_,d,k,B,j=[{type:"radio"},{name:a[1]},{__value:a[2]},a[10](),{tabindex:"-1"}],H={};for(let g=0;g<j.length;g+=1)H=te(H,j[g]);let E=a[11].lead&&pe(a);const M=a[21].default,N=ce(M,a,a[20],null);return d=Qe(a[29][0]),{c(){e=A("label"),n=A("div"),l=A("div"),t=A("input"),s=G(),i=A("div"),E&&E.c(),r=G(),o=A("div"),N&&N.c(),this.h()},l(g){e=L(g,"LABEL",{class:!0});var D=T(e);n=L(D,"DIV",{class:!0,"data-testid":!0,role:!0,"aria-controls":!0,"aria-selected":!0,tabindex:!0});var O=T(n);l=L(O,"DIV",{class:!0});var C=T(l);t=L(C,"INPUT",{type:!0,name:!0,tabindex:!0}),C.forEach(b),s=K(O),i=L(O,"DIV",{class:!0});var m=T(i);E&&E.l(m),r=K(m),o=L(m,"DIV",{class:!0});var re=T(o);N&&N.l(re),re.forEach(b),m.forEach(b),O.forEach(b),D.forEach(b),this.h()},h(){ge(t,H),y(l,"class","h-0 w-0 overflow-hidden"),y(o,"class","tab-label"),y(i,"class",u="tab-interface "+a[7]),y(n,"class",f="tab "+a[6]),y(n,"data-testid","tab"),y(n,"role","tab"),y(n,"aria-controls",a[3]),y(n,"aria-selected",a[4]),y(n,"tabindex",c=a[4]?0:-1),y(e,"class",a[8]),d.p(t)},m(g,D){I(g,e,D),V(e,n),V(n,l),V(l,t),t.autofocus&&t.focus(),a[27](t),t.checked=t.__value===a[0],V(n,s),V(n,i),E&&E.m(i,null),V(i,r),V(i,o),N&&N.m(o,null),_=!0,k||(B=[R(t,"change",a[28]),R(t,"click",a[25]),R(t,"change",a[26]),R(n,"keydown",a[9]),R(n,"keydown",a[22]),R(n,"keyup",a[23]),R(n,"keypress",a[24])],k=!0)},p(g,D){ge(t,H=Ce(j,[{type:"radio"},(!_||D[0]&2)&&{name:g[1]},(!_||D[0]&4)&&{__value:g[2]},g[10](),{tabindex:"-1"}])),D[0]&1&&(t.checked=t.__value===g[0]),g[11].lead?E?(E.p(g,D),D[0]&2048&&v(E,1)):(E=pe(g),E.c(),v(E,1),E.m(i,r)):E&&(W(),p(E,1,1,()=>{E=null}),J()),N&&N.p&&(!_||D[0]&1048576)&&de(N,M,g,g[20],_?be(M,g[20],D,null):_e(g[20]),null),(!_||D[0]&128&&u!==(u="tab-interface "+g[7]))&&y(i,"class",u),(!_||D[0]&64&&f!==(f="tab "+g[6]))&&y(n,"class",f),(!_||D[0]&8)&&y(n,"aria-controls",g[3]),(!_||D[0]&16)&&y(n,"aria-selected",g[4]),(!_||D[0]&16&&c!==(c=g[4]?0:-1))&&y(n,"tabindex",c),(!_||D[0]&256)&&y(e,"class",g[8])},i(g){_||(v(E),v(N,g),_=!0)},o(g){p(E),p(N,g),_=!1},d(g){g&&b(e),a[27](null),E&&E.d(),N&&N.d(g),d.r(),k=!1,je(B)}}}const ft="text-center cursor-pointer transition-colors duration-100",rt="";function ut(a,e,n){let l,t,s,i,r;const o=["group","name","value","controls","regionTab","active","hover","flex","padding","rounded","spacing"];let u=ve(e,o),{$$slots:f={},$$scope:c}=e;const _=qe(f);let{group:d}=e,{name:k}=e,{value:B}=e,{controls:j=""}=e,{regionTab:H=""}=e,{active:E=ee("active")}=e,{hover:M=ee("hover")}=e,{flex:N=ee("flex")}=e,{padding:g=ee("padding")}=e,{rounded:D=ee("rounded")}=e,{spacing:O=ee("spacing")}=e,C;function m(h){if(["Enter","Space"].includes(h.code))h.preventDefault(),C.click();else if(h.code==="ArrowRight"){const ae=C.closest(".tab-list");if(!ae)return;const Q=Array.from(ae.querySelectorAll(".tab")),se=C.closest(".tab");if(!se)return;const oe=Q.indexOf(se),he=oe+1>=Q.length?0:oe+1,U=Q[he],fe=U==null?void 0:U.querySelector("input");U&&fe&&(fe.click(),U.focus())}else if(h.code==="ArrowLeft"){const ae=C.closest(".tab-list");if(!ae)return;const Q=Array.from(ae.querySelectorAll(".tab")),se=C.closest(".tab");if(!se)return;const oe=Q.indexOf(se),he=oe-1<0?Q.length-1:oe-1,U=Q[he],fe=U==null?void 0:U.querySelector("input");U&&fe&&(fe.click(),U.focus())}}function re(){return delete u.class,u}const Ge=[[]];function Ke(h){z.call(this,a,h)}function Me(h){z.call(this,a,h)}function Oe(h){z.call(this,a,h)}function ze(h){z.call(this,a,h)}function Fe(h){z.call(this,a,h)}function We(h){He[h?"unshift":"push"](()=>{C=h,n(5,C)})}function Je(){d=this.__value,n(0,d)}return a.$$set=h=>{n(31,e=te(te({},e),ue(h))),n(30,u=ve(e,o)),"group"in h&&n(0,d=h.group),"name"in h&&n(1,k=h.name),"value"in h&&n(2,B=h.value),"controls"in h&&n(3,j=h.controls),"regionTab"in h&&n(12,H=h.regionTab),"active"in h&&n(13,E=h.active),"hover"in h&&n(14,M=h.hover),"flex"in h&&n(15,N=h.flex),"padding"in h&&n(16,g=h.padding),"rounded"in h&&n(17,D=h.rounded),"spacing"in h&&n(18,O=h.spacing),"$$scope"in h&&n(20,c=h.$$scope)},a.$$.update=()=>{a.$$.dirty[0]&5&&n(4,l=B===d),a.$$.dirty[0]&24592&&n(19,t=l?E:M),n(8,s=`${ft} ${N} ${g} ${D} ${t} ${e.class??""}`),a.$$.dirty[0]&262144&&n(7,i=`${rt} ${O}`),a.$$.dirty[0]&4096&&n(6,r=`${H}`)},e=ue(e),[d,k,B,j,l,C,r,i,s,m,re,_,H,E,M,N,g,D,O,t,c,f,Ke,Me,Oe,ze,Fe,We,Je,Ge]}let ct=class extends le{constructor(e){super(),ne(this,e,ut,ot,ie,{group:0,name:1,value:2,controls:3,regionTab:12,active:13,hover:14,flex:15,padding:16,rounded:17,spacing:18},null,[-1,-1])}};function Ee(a,e,n){const l=a.slice();return l[3]=e[n],l}function De(a){let e,n,l,t,s,i,r=a[0],o=[];for(let u=0;u<r.length;u+=1)o[u]=Ve(Ee(a,r,u));return{c(){e=A("hr"),n=G(),l=A("b"),t=P("debug infos:"),s=G();for(let u=0;u<o.length;u+=1)o[u].c();i=q()},l(u){e=L(u,"HR",{}),n=K(u),l=L(u,"B",{});var f=T(l);t=S(f,"debug infos:"),f.forEach(b),s=K(u);for(let c=0;c<o.length;c+=1)o[c].l(u);i=q()},m(u,f){I(u,e,f),I(u,n,f),I(u,l,f),V(l,t),I(u,s,f);for(let c=0;c<o.length;c+=1)o[c]&&o[c].m(u,f);I(u,i,f)},p(u,f){if(f&1){r=u[0];let c;for(c=0;c<r.length;c+=1){const _=Ee(u,r,c);o[c]?o[c].p(_,f):(o[c]=Ve(_),o[c].c(),o[c].m(i.parentNode,i))}for(;c<o.length;c+=1)o[c].d(1);o.length=r.length}},d(u){u&&b(e),u&&b(n),u&&b(l),u&&b(s),me(o,u),u&&b(i)}}}function Ve(a){let e,n=a[3].name+"",l,t,s=a[3].displayName+"",i,r,o=a[3].status+"",u;return{c(){e=A("p"),l=P(n),t=P("|"),i=P(s),r=P("| Status : "),u=P(o)},l(f){e=L(f,"P",{});var c=T(e);l=S(c,n),t=S(c,"|"),i=S(c,s),r=S(c,"| Status : "),u=S(c,o),c.forEach(b)},m(f,c){I(f,e,c),V(e,l),V(e,t),V(e,i),V(e,r),V(e,u)},p(f,c){c&1&&n!==(n=f[3].name+"")&&F(l,n),c&1&&s!==(s=f[3].displayName+"")&&F(i,s),c&1&&o!==(o=f[3].status+"")&&F(u,o)},d(f){f&&b(e)}}}function dt(a){let e,n,l,t,s,i,r,o=a[0]&&a[1]&&De(a);return{c(){e=A("label"),n=A("input"),l=P(`\r
	show debug information`),t=G(),o&&o.c(),s=q(),this.h()},l(u){e=L(u,"LABEL",{});var f=T(e);n=L(f,"INPUT",{type:!0}),l=S(f,`\r
	show debug information`),f.forEach(b),t=K(u),o&&o.l(u),s=q(),this.h()},h(){y(n,"type","checkbox")},m(u,f){I(u,e,f),V(e,n),n.checked=a[1],V(e,l),I(u,t,f),o&&o.m(u,f),I(u,s,f),i||(r=R(n,"change",a[2]),i=!0)},p(u,[f]){f&2&&(n.checked=u[1]),u[0]&&u[1]?o?o.p(u,f):(o=De(u),o.c(),o.m(s.parentNode,s)):o&&(o.d(1),o=null)},i:X,o:X,d(u){u&&b(e),u&&b(t),o&&o.d(u),u&&b(s),i=!1,r()}}}function _t(a,e,n){let{hooks:l}=e,t=!1;function s(){t=this.checked,n(1,t)}return a.$$set=i=>{"hooks"in i&&n(0,l=i.hooks)},[l,t,s]}class bt extends le{constructor(e){super(),ne(this,e,_t,dt,ie,{hooks:0})}}const ht=async a=>{try{return(await we.get("/dcm/view/load?id="+a)).data}catch(e){console.error(e)}};function mt(a){let e,n,l,t,s,i,r,o,u,f,c,_;return{c(){e=A("h2"),n=P(a[2]),l=P(`\r
Dataset Id: `),t=P(a[0]),s=P(`\r
Version: `),i=P(a[1]),r=G(),o=A("b"),u=P("version selection"),f=G(),c=A("b"),_=P("download"),this.h()},l(d){e=L(d,"H2",{class:!0});var k=T(e);n=S(k,a[2]),k.forEach(b),l=S(d,`\r
Dataset Id: `),t=S(d,a[0]),s=S(d,`\r
Version: `),i=S(d,a[1]),r=K(d),o=L(d,"B",{});var B=T(o);u=S(B,"version selection"),B.forEach(b),f=K(d),c=L(d,"B",{});var j=T(c);_=S(j,"download"),j.forEach(b),this.h()},h(){y(e,"class","h2")},m(d,k){I(d,e,k),V(e,n),I(d,l,k),I(d,t,k),I(d,s,k),I(d,i,k),I(d,r,k),I(d,o,k),V(o,u),I(d,f,k),I(d,c,k),V(c,_)},p(d,[k]){k&4&&F(n,d[2]),k&1&&F(t,d[0]),k&2&&F(i,d[1])},i:X,o:X,d(d){d&&b(e),d&&b(l),d&&b(t),d&&b(s),d&&b(i),d&&b(r),d&&b(o),d&&b(f),d&&b(c)}}}function gt(a,e,n){let{id:l}=e,{version:t}=e,{title:s=""}=e;return a.$$set=i=>{"id"in i&&n(0,l=i.id),"version"in i&&n(1,t=i.version),"title"in i&&n(2,s=i.title)},[l,t,s]}class vt extends le{constructor(e){super(),ne(this,e,gt,mt,ie,{id:0,version:1,title:2})}}function Ae(a){let e,n,l,t,s,i;const r=[yt,kt],o=[];function u(f,c){return f[3]?0:1}return t=u(a),s=o[t]=r[t](a),{c(){e=P(a[1]),n=G(),l=A("div"),s.c(),this.h()},l(f){e=S(f,a[1]),n=K(f),l=L(f,"DIV",{id:!0});var c=T(l);s.l(c),c.forEach(b),this.h()},h(){y(l,"id",a[0])},m(f,c){I(f,e,c),I(f,n,c),I(f,l,c),o[t].m(l,null),i=!0},p(f,c){(!i||c&2)&&F(e,f[1]);let _=t;t=u(f),t===_?o[t].p(f,c):(W(),p(o[_],1,1,()=>{o[_]=null}),J(),s=o[t],s?s.p(f,c):(s=o[t]=r[t](f),s.c()),v(s,1),s.m(l,null)),(!i||c&1)&&y(l,"id",f[0])},i(f){i||(v(s),i=!0)},o(f){p(s),i=!1},d(f){f&&b(e),f&&b(n),f&&b(l),o[t].d()}}}function kt(a){let e,n;return e=new Ue({}),{c(){Y(e.$$.fragment)},l(l){Z(e.$$.fragment,l)},m(l,t){w(e,l,t),n=!0},p:X,i(l){n||(v(e.$$.fragment,l),n=!0)},o(l){p(e.$$.fragment,l),n=!1},d(l){x(e,l)}}}function yt(a){let e,n;return{c(){e=A("div"),n=P(a[1])},l(l){e=L(l,"DIV",{});var t=T(e);n=S(t,a[1]),t.forEach(b)},m(l,t){I(l,e,t),V(e,n)},p(l,t){t&2&&F(n,l[1])},i:X,o:X,d(l){l&&b(e)}}}function It(a){let e,n,l=a[2]&&Ae(a);return{c(){l&&l.c(),e=q()},l(t){l&&l.l(t),e=q()},m(t,s){l&&l.m(t,s),I(t,e,s),n=!0},p(t,[s]){t[2]?l?(l.p(t,s),s&4&&v(l,1)):(l=Ae(t),l.c(),v(l,1),l.m(e.parentNode,e)):l&&(W(),p(l,1,1,()=>{l=null}),J())},i(t){n||(v(l),n=!0)},o(t){p(l),n=!1},d(t){l&&l.d(t),t&&b(e)}}}function pt(a,e,n){let l,{id:t}=e,{version:s}=e,{name:i}=e,{displayName:r}=e,{start:o}=e,{active:u}=e;return Re(async()=>{console.log(i)}),a.$$set=f=>{"id"in f&&n(4,t=f.id),"version"in f&&n(5,s=f.version),"name"in f&&n(0,i=f.name),"displayName"in f&&n(1,r=f.displayName),"start"in f&&n(6,o=f.start),"active"in f&&n(2,u=f.active)},n(3,l=""),[i,r,u,l,t,s,o]}class Et extends le{constructor(e){super(),ne(this,e,pt,It,ie,{id:4,version:5,name:0,displayName:1,start:6,active:2})}}function Le(a,e,n){const l=a.slice();return l[11]=e[n],l[13]=n,l}function Ne(a,e,n){const l=a.slice();return l[11]=e[n],l[13]=n,l}function Dt(a){let e,n,l;return n=new Ue({props:{label:"loading dataset "+a[0]}}),{c(){e=A("div"),Y(n.$$.fragment)},l(t){e=L(t,"DIV",{});var s=T(e);Z(n.$$.fragment,s),s.forEach(b)},m(t,s){I(t,e,s),w(n,e,null),l=!0},p(t,s){const i={};s&1&&(i.label="loading dataset "+t[0]),n.$set(i)},i(t){l||(v(n.$$.fragment,t),l=!0)},o(t){p(n.$$.fragment,t),l=!1},d(t){t&&b(e),x(n)}}}function Vt(a){let e,n;return e=new at({props:{$$slots:{panel:[Nt],default:[Lt]},$$scope:{ctx:a}}}),{c(){Y(e.$$.fragment)},l(l){Z(e.$$.fragment,l)},m(l,t){w(e,l,t),n=!0},p(l,t){const s={};t&32798&&(s.$$scope={dirty:t,ctx:l}),e.$set(s)},i(l){n||(v(e.$$.fragment,l),n=!0)},o(l){p(e.$$.fragment,l),n=!1},d(l){x(e,l)}}}function Te(a){let e,n,l;function t(i){a[5](i)}let s={name:a[11].name,value:a[13],$$slots:{default:[At]},$$scope:{ctx:a}};return a[3]!==void 0&&(s.group=a[3]),e=new ct({props:s}),He.push(()=>Xe(e,"group",t)),{c(){Y(e.$$.fragment)},l(i){Z(e.$$.fragment,i)},m(i,r){w(e,i,r),l=!0},p(i,r){const o={};r&16&&(o.name=i[11].name),r&32784&&(o.$$scope={dirty:r,ctx:i}),!n&&r&8&&(n=!0,o.group=i[3],Ye(()=>n=!1)),e.$set(o)},i(i){l||(v(e.$$.fragment,i),l=!0)},o(i){p(e.$$.fragment,i),l=!1},d(i){x(e,i)}}}function At(a){let e=a[11].name+"",n;return{c(){n=P(e)},l(l){n=S(l,e)},m(l,t){I(l,n,t)},p(l,t){t&16&&e!==(e=l[11].name+"")&&F(n,e)},d(l){l&&b(n)}}}function Be(a){let e,n,l=a[11].status==2&&Te(a);return{c(){l&&l.c(),e=q()},l(t){l&&l.l(t),e=q()},m(t,s){l&&l.m(t,s),I(t,e,s),n=!0},p(t,s){t[11].status==2?l?(l.p(t,s),s&16&&v(l,1)):(l=Te(t),l.c(),v(l,1),l.m(e.parentNode,e)):l&&(W(),p(l,1,1,()=>{l=null}),J())},i(t){n||(v(l),n=!0)},o(t){p(l),n=!1},d(t){l&&l.d(t),t&&b(e)}}}function Lt(a){let e,n,l=a[4],t=[];for(let i=0;i<l.length;i+=1)t[i]=Be(Le(a,l,i));const s=i=>p(t[i],1,1,()=>{t[i]=null});return{c(){for(let i=0;i<t.length;i+=1)t[i].c();e=q()},l(i){for(let r=0;r<t.length;r+=1)t[r].l(i);e=q()},m(i,r){for(let o=0;o<t.length;o+=1)t[o]&&t[o].m(i,r);I(i,e,r),n=!0},p(i,r){if(r&24){l=i[4];let o;for(o=0;o<l.length;o+=1){const u=Le(i,l,o);t[o]?(t[o].p(u,r),v(t[o],1)):(t[o]=Be(u),t[o].c(),v(t[o],1),t[o].m(e.parentNode,e))}for(W(),o=l.length;o<t.length;o+=1)s(o);J()}},i(i){if(!n){for(let r=0;r<l.length;r+=1)v(t[r]);n=!0}},o(i){t=t.filter(Boolean);for(let r=0;r<t.length;r+=1)p(t[r]);n=!1},d(i){me(t,i),i&&b(e)}}}function Pe(a){let e,n;const l=[{id:a[1]},{version:a[2]},a[11],{active:a[3]==a[13]}];let t={};for(let s=0;s<l.length;s+=1)t=te(t,l[s]);return e=new Et({props:t}),{c(){Y(e.$$.fragment)},l(s){Z(e.$$.fragment,s)},m(s,i){w(e,s,i),n=!0},p(s,i){const r=i&30?Ce(l,[i&2&&{id:s[1]},i&4&&{version:s[2]},i&16&&Ze(s[11]),i&8&&{active:s[3]==s[13]}]):{};e.$set(r)},i(s){n||(v(e.$$.fragment,s),n=!0)},o(s){p(e.$$.fragment,s),n=!1},d(s){x(e,s)}}}function Se(a){let e,n,l=a[11].status==2&&Pe(a);return{c(){l&&l.c(),e=q()},l(t){l&&l.l(t),e=q()},m(t,s){l&&l.m(t,s),I(t,e,s),n=!0},p(t,s){t[11].status==2?l?(l.p(t,s),s&16&&v(l,1)):(l=Pe(t),l.c(),v(l,1),l.m(e.parentNode,e)):l&&(W(),p(l,1,1,()=>{l=null}),J())},i(t){n||(v(l),n=!0)},o(t){p(l),n=!1},d(t){l&&l.d(t),t&&b(e)}}}function Nt(a){let e,n,l=a[4],t=[];for(let i=0;i<l.length;i+=1)t[i]=Se(Ne(a,l,i));const s=i=>p(t[i],1,1,()=>{t[i]=null});return{c(){for(let i=0;i<t.length;i+=1)t[i].c();e=q()},l(i){for(let r=0;r<t.length;r+=1)t[r].l(i);e=q()},m(i,r){for(let o=0;o<t.length;o+=1)t[o]&&t[o].m(i,r);I(i,e,r),n=!0},p(i,r){if(r&30){l=i[4];let o;for(o=0;o<l.length;o+=1){const u=Ne(i,l,o);t[o]?(t[o].p(u,r),v(t[o],1)):(t[o]=Se(u),t[o].c(),v(t[o],1),t[o].m(e.parentNode,e))}for(W(),o=l.length;o<t.length;o+=1)s(o);J()}},i(i){if(!n){for(let r=0;r<l.length;r+=1)v(t[r]);n=!0}},o(i){t=t.filter(Boolean);for(let r=0;r<t.length;r+=1)p(t[r]);n=!1},d(i){me(t,i),i&&b(e)}}}function Tt(a){let e,n,l,t,s,i,r,o;n=new vt({props:{id:a[1],version:a[2],title:a[0]}});const u=[Vt,Dt],f=[];function c(_,d){return _[4]?0:1}return t=c(a),s=f[t]=u[t](a),r=new bt({props:{hooks:a[4]}}),{c(){e=A("div"),Y(n.$$.fragment),l=G(),s.c(),i=G(),Y(r.$$.fragment)},l(_){e=L(_,"DIV",{});var d=T(e);Z(n.$$.fragment,d),l=K(d),s.l(d),i=K(d),Z(r.$$.fragment,d),d.forEach(b)},m(_,d){I(_,e,d),w(n,e,null),V(e,l),f[t].m(e,null),V(e,i),w(r,e,null),o=!0},p(_,[d]){const k={};d&2&&(k.id=_[1]),d&4&&(k.version=_[2]),d&1&&(k.title=_[0]),n.$set(k);let B=t;t=c(_),t===B?f[t].p(_,d):(W(),p(f[B],1,1,()=>{f[B]=null}),J(),s=f[t],s?s.p(_,d):(s=f[t]=u[t](_),s.c()),v(s,1),s.m(e,i));const j={};d&16&&(j.hooks=_[4]),r.$set(j)},i(_){o||(v(n.$$.fragment,_),v(s),v(r.$$.fragment,_),o=!0)},o(_){p(n.$$.fragment,_),p(s),p(r.$$.fragment,_),o=!1},d(_){_&&b(e),x(n),f[t].d(),x(r)}}}function Bt(a,e,n){let l,t="",s,i,r=0,o,u=0,f;Re(async()=>{s=document.getElementById("view"),n(1,i=s==null?void 0:s.getAttribute("dataset")),n(2,r=s==null?void 0:s.getAttribute("version")),console.log("start view",i,r),xe("https://localhost:44345","davidschoene","123456"),o=await ht(i),console.log("onmount",o),n(4,l=o.hooks),n(0,t=o.title),n(2,r=o.version),n(1,i=o.id),console.log(o),console.log("hooks",l)});function c(_){u=_,n(3,u)}return n(4,l=f),[t,i,r,u,l,c]}class Ct extends le{constructor(e){super(),ne(this,e,Bt,Tt,ie,{})}}export{Ct as component};
