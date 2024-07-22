import{s as Oe,e as E,l as G,c as y,h as F,d as m,o as H,a as p,i as N,p as $,x as X,L as xe,B as qe,Q as Ve,n as ve,j as be,t as se,m as oe,q as _e,O as ye,R as bt,S as kt,F as Y,P as Z,E as Ze,H as Ae,g as $t}from"../chunks/scheduler.BD3IPErK.js";import{M as wt,h as Et,u as yt,E as Lt}from"../chunks/ErrorMessage.C_-DmavX.js";import{S as Me,i as Ue,c as A,a as M,m as U,t as L,b as C,d as B,g as he,e as ge,f as ee,h as et,j as Ge,k as He}from"../chunks/index.CufQnT-K.js";import{g as Tt}from"../chunks/stores.CsfdB5nm.js";import{A as fe,a as Le,n as Ce,b as Ie}from"../chunks/eslint4b.es.C89pjaTy.js";import{F as ue,l as Ft,f as St,a as Vt,k as Ct,c as It,P as Dt,h as ft,d as ke,s as tt}from"../chunks/Page.DNGIaKrR.js";import{e as De,a as rt}from"../chunks/types.eQO6CGud.js";import{T as Pt,a as Nt}from"../chunks/TablePlaceholder.Cg2Oo5AA.js";import{e as ie,a as ct,p as dt,b as pt}from"../chunks/stores.BS-gZ-KN.js";import{M as je}from"../chunks/MultiSelect.DlYEx5Wv.js";import{y as xt,p as Ot,P as Pe,v as Ne,T as nt}from"../chunks/vest.production.DBLC5iAh.js";const At=async()=>{try{return(await fe.get("/rpm/ExternalLink/get")).data}catch(r){throw console.error(r),r}},Mt=async()=>{try{return(await fe.get("/rpm/ExternalLink/getLinkTypes")).data}catch(r){throw console.error(r),r}},Ut=async()=>{try{return(await fe.get("/rpm/ExternalLink/GetPrefixCategoriesAsList")).data}catch(r){throw console.error(r),r}},Bt=async()=>{try{return(await fe.get("/rpm/ExternalLink/GetPrefixListItems")).data}catch(r){throw console.error(r),r}},qt=async r=>{try{return await fe.delete("/rpm/ExternalLink/delete?id="+r,null)}catch(e){throw console.error(e),e}},Gt=async r=>{try{return console.log("🚀 ~ file: services.ts:38 ~ create ~ data:",r),await fe.post("/rpm/ExternalLink/create",r)}catch(e){throw console.error(e),e}},Ht=async r=>{try{return await fe.post("/rpm/ExternalLink/update",r)}catch(e){throw console.error(e),e}};function jt(r){let e,t,n,i,l,s,o,a,u,c,v,T,k,S,b,_,d,w,D;return i=new ue({props:{icon:Ft}}),u=new ue({props:{icon:St}}),S=new ue({props:{icon:Vt}}),{c(){e=E("tableOption"),t=E("div"),n=E("button"),A(i.$$.fragment),o=G(),a=E("button"),A(u.$$.fragment),T=G(),k=E("button"),A(S.$$.fragment),this.h()},l(V){e=y(V,"TABLEOPTION",{});var f=F(e);t=y(f,"DIV",{class:!0});var I=F(t);n=y(I,"BUTTON",{type:!0,class:!0,title:!0,id:!0});var W=F(n);M(i.$$.fragment,W),W.forEach(m),o=H(I),a=y(I,"BUTTON",{type:!0,class:!0,title:!0,id:!0});var te=F(a);M(u.$$.fragment,te),te.forEach(m),T=H(I),k=y(I,"BUTTON",{type:!0,class:!0,title:!0,id:!0});var R=F(k);M(S.$$.fragment,R),R.forEach(m),I.forEach(m),f.forEach(m),this.h()},h(){p(n,"type","button"),p(n,"class","chip variant-filled-primary shadow-md"),p(n,"title",l="Link, "+r[0].name),p(n,"id",s="link-"+r[0].id),p(a,"type","button"),p(a,"class","chip variant-filled-primary shadow-md"),p(a,"title",c="Edit External Link, "+r[0].name),p(a,"id",v="edit-"+r[0].id),p(k,"type","button"),p(k,"class","chip variant-filled-error shadow-md"),p(k,"title",b="Delete External Link, "+r[0].name),p(k,"id",_="delete-"+r[0].id),p(t,"class","w-18")},m(V,f){N(V,e,f),$(e,t),$(t,n),U(i,n,null),$(t,o),$(t,a),U(u,a,null),$(t,T),$(t,k),U(S,k,null),d=!0,w||(D=[X(n,"mouseover",r[3]),X(n,"click",xe(r[4])),X(a,"mouseover",r[5]),X(a,"click",xe(r[6])),X(k,"mouseover",r[7]),X(k,"click",xe(r[8]))],w=!0)},p(V,[f]){(!d||f&1&&l!==(l="Link, "+V[0].name))&&p(n,"title",l),(!d||f&1&&s!==(s="link-"+V[0].id))&&p(n,"id",s),(!d||f&1&&c!==(c="Edit External Link, "+V[0].name))&&p(a,"title",c),(!d||f&1&&v!==(v="edit-"+V[0].id))&&p(a,"id",v),(!d||f&1&&b!==(b="Delete External Link, "+V[0].name))&&p(k,"title",b),(!d||f&1&&_!==(_="delete-"+V[0].id))&&p(k,"id",_)},i(V){d||(L(i.$$.fragment,V),L(u.$$.fragment,V),L(S.$$.fragment,V),d=!0)},o(V){C(i.$$.fragment,V),C(u.$$.fragment,V),C(S.$$.fragment,V),d=!1},d(V){V&&m(e),B(i),B(u),B(S),w=!1,qe(D)}}}function Wt(r,e,t){let{row:n}=e,{dispatchFn:i}=e,l="";n.uri!=""&&(l=n.uri,n.prefix&&(l=""+n.prefix.url+n.uri));const s=()=>{Le.show("link")},o=()=>i({type:{action:"link",url:l}}),a=()=>{Le.show("edit")},u=()=>i({type:{action:"edit",id:n.id}}),c=()=>{Le.show("delete")},v=()=>i({type:{action:"delete",id:n.id}});return r.$$set=T=>{"row"in T&&t(0,n=T.row),"dispatchFn"in T&&t(1,i=T.dispatchFn)},[n,i,l,s,o,a,u,c,v]}class Rt extends Me{constructor(e){super(),Ue(this,e,Wt,jt,Oe,{row:0,dispatchFn:1})}}const ae=xt((r={},e)=>{Ot(e),Pe("name","name is required",()=>{Ne(r.name).isNotBlank()}),Pe("name","name allready exist.",()=>{const t=Ve(ie).map(l=>l.name),n=r.id>0?Ve(ie).find(l=>l.id==r.id):{id:0,name:""},i=n?t.filter(l=>l!=n.name):t;Ne(r.name).notInside(i)}),Pe("uri","uri is required",()=>{Ne(r.uri).isNotBlank()}),Pe("uri","uri allready exist.",()=>{const t=r.id>0?Ve(ie).find(i=>i.id==r.id):{id:0,name:"",type:"",uri:""},n=Ve(ie).map(i=>i.uri!=(t==null?void 0:t.uri)?i.uri:"");Ne(r.uri).notInside(n)})}),Qt=[{id:"create",name:"Create new external link",description:"Click here to create a new external link"},{id:"delete",name:"Delete external link",description:"Click here to delete the external link"},{id:"edit",name:"Edit external link",description:"Click here to edit the external link"},{id:"save",name:"Save external link",description:"Click here to save the external link"},{id:"cancel",name:"Cancel",description:"Click here to cancel"},{id:"name",name:"Name",description:"Name of the external link. <li>Has to be unique.</li>"},{id:"uri",name:"Uri",description:"The destination of the external link on the internet."},{id:"type",name:"Type",description:"Type is a context for a group of external links for a better selection of meanings."}];function it(r){let e;function t(l,s){var o;return(o=l[0].prefix)!=null&&o.id?zt:Xt}let n=t(r),i=n(r);return{c(){i.c(),e=be()},l(l){i.l(l),e=be()},m(l,s){i.m(l,s),N(l,e,s)},p(l,s){n===(n=t(l))&&i?i.p(l,s):(i.d(1),i=n(l),i&&(i.c(),i.m(e.parentNode,e)))},d(l){l&&m(e),i.d(l)}}}function Xt(r){let e,t=r[0].uri+"",n;return{c(){e=E("a"),n=se(t),this.h()},l(i){e=y(i,"A",{class:!0,target:!0});var l=F(e);n=oe(l,t),l.forEach(m),this.h()},h(){p(e,"class","a"),p(e,"target","_blank")},m(i,l){N(i,e,l),$(e,n)},p(i,l){l&1&&t!==(t=i[0].uri+"")&&_e(n,t)},d(i){i&&m(e)}}}function zt(r){let e,t,n=r[0].prefix.text+"",i,l,s=r[0].uri+"",o,a,u,c,v=r[0].prefix.url+"",T,k=r[0].uri+"",S,b;return{c(){e=E("div"),t=E("b"),i=se(n),l=se(":"),o=se(s),a=G(),u=E("div"),c=E("a"),T=se(v),S=se(k),this.h()},l(_){e=y(_,"DIV",{});var d=F(e);t=y(d,"B",{class:!0});var w=F(t);i=oe(w,n),l=oe(w,":"),o=oe(w,s),w.forEach(m),d.forEach(m),a=H(_),u=y(_,"DIV",{});var D=F(u);c=y(D,"A",{class:!0,target:!0,href:!0});var V=F(c);T=oe(V,v),S=oe(V,k),V.forEach(m),D.forEach(m),this.h()},h(){p(t,"class","text-sm"),p(c,"class","a"),p(c,"target","_blank"),p(c,"href",b=""+(r[0].prefix.url+r[0].uri))},m(_,d){N(_,e,d),$(e,t),$(t,i),$(t,l),$(t,o),N(_,a,d),N(_,u,d),$(u,c),$(c,T),$(c,S)},p(_,d){d&1&&n!==(n=_[0].prefix.text+"")&&_e(i,n),d&1&&s!==(s=_[0].uri+"")&&_e(o,s),d&1&&v!==(v=_[0].prefix.url+"")&&_e(T,v),d&1&&k!==(k=_[0].uri+"")&&_e(S,k),d&1&&b!==(b=""+(_[0].prefix.url+_[0].uri))&&p(c,"href",b)},d(_){_&&(m(e),m(a),m(u))}}}function Jt(r){let e,t=r[0].uri!=""&&it(r);return{c(){e=E("div"),t&&t.c(),this.h()},l(n){e=y(n,"DIV",{class:!0});var i=F(e);t&&t.l(i),i.forEach(m),this.h()},h(){p(e,"class","card p-5 flex-col gap-2")},m(n,i){N(n,e,i),t&&t.m(e,null)},p(n,[i]){n[0].uri!=""?t?t.p(n,i):(t=it(n),t.c(),t.m(e,null)):t&&(t.d(1),t=null)},i:ve,o:ve,d(n){n&&m(e),t&&t.d()}}}function Kt(r,e,t){let{link:n}=e;return console.log("🚀 ~ file: UrlPreview.svelte:6 ~ link:",n),r.$$set=i=>{"link"in i&&t(0,n=i.link)},[n]}class Yt extends Me{constructor(e){super(),Ue(this,e,Kt,Jt,Oe,{link:0})}}function lt(r){var Xe,ze;let e,t,n,i,l,s,o,a,u,c,v,T,k,S,b,_,d,w,D,V,f,I,W,te,R,Q,z,$e,j,J,ce,de,pe,K,h,me;function mt(g){r[11](g)}let We={id:"name",placeholder:"Name",valid:r[1].isValid("name"),invalid:r[1].hasErrors("name"),feedback:r[1].getErrors("name"),help:Te};r[0].name!==void 0&&(We.value=r[0].name),l=new nt({props:We}),Y.push(()=>ee(l,"value",mt)),l.$on("change",r[12]),l.$on("input",r[7]);function _t(g){r[13](g)}function ht(g){r[14](g)}let Be={id:"type",title:"Type",itemId:"id",itemLabel:"text",itemGroup:"group",complexSource:!0,complexTarget:!0,isMulti:!1,clearable:!1,placeholder:"-- Please select --",invalid:r[1].hasErrors("type"),feedback:r[1].getErrors("type"),help:Te};r[2]!==void 0&&(Be.source=r[2]),r[0].type!==void 0&&(Be.target=r[0].type),c=new je({props:Be}),Y.push(()=>ee(c,"source",_t)),Y.push(()=>ee(c,"target",ht)),c.$on("change",r[15]);let x=((Xe=r[0].type)==null?void 0:Xe.id)===De.prefix&&at(r),O=((ze=r[0].type)==null?void 0:ze.id)!==De.prefix&&st(r);function gt(g){r[22](g)}let Re={id:"uri",label:"Uri",placeholder:"Uri",valid:r[1].isValid("uri"),invalid:r[1].hasErrors("uri"),feedback:r[1].getErrors("uri"),help:Te};r[0].uri!==void 0&&(Re.value=r[0].uri),D=new nt({props:Re}),Y.push(()=>ee(D,"value",gt)),D.$on("change",r[23]),D.$on("input",r[7]);function vt(g){r[24](g)}let Qe={};return r[0]!==void 0&&(Qe.link=r[0]),I=new Yt({props:Qe}),Y.push(()=>ee(I,"link",vt)),z=new ue({props:{icon:Ct}}),J=new ue({props:{icon:It}}),{c(){e=E("form"),t=E("div"),n=E("div"),i=E("div"),A(l.$$.fragment),o=G(),a=E("div"),u=E("div"),A(c.$$.fragment),k=G(),S=E("div"),x&&x.c(),b=G(),_=E("div"),O&&O.c(),d=G(),w=E("div"),A(D.$$.fragment),f=G(),A(I.$$.fragment),te=G(),R=E("div"),Q=E("button"),A(z.$$.fragment),$e=G(),j=E("button"),A(J.$$.fragment),this.h()},l(g){e=y(g,"FORM",{});var P=F(e);t=y(P,"DIV",{id:!0,class:!0});var q=F(t);n=y(q,"DIV",{class:!0});var re=F(n);i=y(re,"DIV",{class:!0});var ne=F(i);M(l.$$.fragment,ne),ne.forEach(m),re.forEach(m),o=H(q),a=y(q,"DIV",{class:!0});var le=F(a);u=y(le,"DIV",{class:!0});var we=F(u);M(c.$$.fragment,we),we.forEach(m),k=H(le),S=y(le,"DIV",{class:!0});var Ee=F(S);x&&x.l(Ee),Ee.forEach(m),le.forEach(m),b=H(q),_=y(q,"DIV",{class:!0});var Fe=F(_);O&&O.l(Fe),d=H(Fe),w=y(Fe,"DIV",{class:!0});var Je=F(w);M(D.$$.fragment,Je),Je.forEach(m),Fe.forEach(m),f=H(q),M(I.$$.fragment,q),te=H(q),R=y(q,"DIV",{class:!0});var Se=F(R);Q=y(Se,"BUTTON",{type:!0,class:!0,title:!0,id:!0});var Ke=F(Q);M(z.$$.fragment,Ke),Ke.forEach(m),$e=H(Se),j=y(Se,"BUTTON",{type:!0,class:!0,title:!0,id:!0});var Ye=F(j);M(J.$$.fragment,Ye),Ye.forEach(m),Se.forEach(m),q.forEach(m),P.forEach(m),this.h()},h(){p(i,"class","w-1/2"),p(n,"class","flex gap-5 items-center"),p(u,"class","w-1/4"),p(S,"class","w-1/4"),p(a,"class","flex gap-5 items-center"),p(w,"class","grow"),p(_,"class","flex gap-5"),p(Q,"type","button"),p(Q,"class","btn variant-filled-warning h-9 w-16 shadow-md"),p(Q,"title","Cancel"),p(Q,"id","cancel"),p(j,"type","submit"),p(j,"class","btn variant-filled-primary h-9 w-16 shadow-md"),p(j,"title",ce="Save external link, "+r[0].name),p(j,"id","save"),j.disabled=de=!r[6],p(R,"class","py-5 text-right col-span-2"),p(t,"id",pe="link-"+r[0].id+"-form"),p(t,"class","space-y-5 card shadow-md p-5")},m(g,P){N(g,e,P),$(e,t),$(t,n),$(n,i),U(l,i,null),$(t,o),$(t,a),$(a,u),U(c,u,null),$(a,k),$(a,S),x&&x.m(S,null),$(t,b),$(t,_),O&&O.m(_,null),$(_,d),$(_,w),U(D,w,null),$(t,f),U(I,t,null),$(t,te),$(t,R),$(R,Q),U(z,Q,null),$(R,$e),$(R,j),U(J,j,null),K=!0,h||(me=[X(Q,"click",r[25]),X(e,"submit",xe(r[9]))],h=!0)},p(g,P){var we,Ee;const q={};P[0]&2&&(q.valid=g[1].isValid("name")),P[0]&2&&(q.invalid=g[1].hasErrors("name")),P[0]&2&&(q.feedback=g[1].getErrors("name")),!s&&P[0]&1&&(s=!0,q.value=g[0].name,Z(()=>s=!1)),l.$set(q);const re={};P[0]&2&&(re.invalid=g[1].hasErrors("type")),P[0]&2&&(re.feedback=g[1].getErrors("type")),!v&&P[0]&4&&(v=!0,re.source=g[2],Z(()=>v=!1)),!T&&P[0]&1&&(T=!0,re.target=g[0].type,Z(()=>T=!1)),c.$set(re),((we=g[0].type)==null?void 0:we.id)===De.prefix?x?(x.p(g,P),P[0]&1&&L(x,1)):(x=at(g),x.c(),L(x,1),x.m(S,null)):x&&(he(),C(x,1,1,()=>{x=null}),ge()),((Ee=g[0].type)==null?void 0:Ee.id)!==De.prefix?O?(O.p(g,P),P[0]&1&&L(O,1)):(O=st(g),O.c(),L(O,1),O.m(_,d)):O&&(he(),C(O,1,1,()=>{O=null}),ge());const ne={};P[0]&2&&(ne.valid=g[1].isValid("uri")),P[0]&2&&(ne.invalid=g[1].hasErrors("uri")),P[0]&2&&(ne.feedback=g[1].getErrors("uri")),!V&&P[0]&1&&(V=!0,ne.value=g[0].uri,Z(()=>V=!1)),D.$set(ne);const le={};!W&&P[0]&1&&(W=!0,le.link=g[0],Z(()=>W=!1)),I.$set(le),(!K||P[0]&1&&ce!==(ce="Save external link, "+g[0].name))&&p(j,"title",ce),(!K||P[0]&64&&de!==(de=!g[6]))&&(j.disabled=de),(!K||P[0]&1&&pe!==(pe="link-"+g[0].id+"-form"))&&p(t,"id",pe)},i(g){K||(L(l.$$.fragment,g),L(c.$$.fragment,g),L(x),L(O),L(D.$$.fragment,g),L(I.$$.fragment,g),L(z.$$.fragment,g),L(J.$$.fragment,g),K=!0)},o(g){C(l.$$.fragment,g),C(c.$$.fragment,g),C(x),C(O),C(D.$$.fragment,g),C(I.$$.fragment,g),C(z.$$.fragment,g),C(J.$$.fragment,g),K=!1},d(g){g&&m(e),B(l),B(c),x&&x.d(),O&&O.d(),B(D),B(I),B(z),B(J),h=!1,qe(me)}}}function at(r){let e,t,n,i;function l(a){r[16](a)}function s(a){r[17](a)}let o={id:"prefixCategory",title:"Prefix Category",itemId:"id",itemLabel:"name",itemGroup:"group",complexSource:!0,complexTarget:!0,isMulti:!1,clearable:!0,placeholder:"-- Please select --",invalid:r[1].hasErrors("type"),feedback:r[1].getErrors("type"),help:Te};return r[4]!==void 0&&(o.source=r[4]),r[0].prefixCategory!==void 0&&(o.target=r[0].prefixCategory),e=new je({props:o}),Y.push(()=>ee(e,"source",l)),Y.push(()=>ee(e,"target",s)),e.$on("change",r[18]),{c(){A(e.$$.fragment)},l(a){M(e.$$.fragment,a)},m(a,u){U(e,a,u),i=!0},p(a,u){const c={};u[0]&2&&(c.invalid=a[1].hasErrors("type")),u[0]&2&&(c.feedback=a[1].getErrors("type")),!t&&u[0]&16&&(t=!0,c.source=a[4],Z(()=>t=!1)),!n&&u[0]&1&&(n=!0,c.target=a[0].prefixCategory,Z(()=>n=!1)),e.$set(c)},i(a){i||(L(e.$$.fragment,a),i=!0)},o(a){C(e.$$.fragment,a),i=!1},d(a){B(e,a)}}}function st(r){let e,t,n,i,l;function s(u){r[19](u)}function o(u){r[20](u)}let a={id:"prefix",title:"Prefix",itemId:"id",itemLabel:"text",itemGroup:"group",complexSource:!0,complexTarget:!0,isMulti:!1,clearable:!0,placeholder:"-- Please select --",invalid:r[1].hasErrors("type"),feedback:r[1].getErrors("type"),help:Te};return r[5]!==void 0&&(a.source=r[5]),r[0].prefix!==void 0&&(a.target=r[0].prefix),t=new je({props:a}),Y.push(()=>ee(t,"source",s)),Y.push(()=>ee(t,"target",o)),t.$on("change",r[21]),{c(){e=E("div"),A(t.$$.fragment),this.h()},l(u){e=y(u,"DIV",{class:!0});var c=F(e);M(t.$$.fragment,c),c.forEach(m),this.h()},h(){p(e,"class","w-1/4")},m(u,c){N(u,e,c),U(t,e,null),l=!0},p(u,c){const v={};c[0]&2&&(v.invalid=u[1].hasErrors("type")),c[0]&2&&(v.feedback=u[1].getErrors("type")),!n&&c[0]&32&&(n=!0,v.source=u[5],Z(()=>n=!1)),!i&&c[0]&1&&(i=!0,v.target=u[0].prefix,Z(()=>i=!1)),t.$set(v)},i(u){l||(L(t.$$.fragment,u),l=!0)},o(u){C(t.$$.fragment,u),l=!1},d(u){u&&m(e),B(t)}}}function Zt(r){let e,t,n=r[3]&&lt(r);return{c(){n&&n.c(),e=be()},l(i){n&&n.l(i),e=be()},m(i,l){n&&n.m(i,l),N(i,e,l),t=!0},p(i,l){i[3]?n?(n.p(i,l),l[0]&8&&L(n,1)):(n=lt(i),n.c(),L(n,1),n.m(e.parentNode,e)):n&&(he(),C(n,1,1,()=>{n=null}),ge())},i(i){t||(L(n),t=!0)},o(i){C(n),t=!1},d(i){i&&m(e),n&&n.d(i)}}}let Te=!0;function er(r,e,t){let n,i,l,s,o;ye(r,ct,h=>t(26,i=h)),ye(r,dt,h=>t(27,l=h)),ye(r,pt,h=>t(28,s=h)),ye(r,ie,h=>t(29,o=h));let{link:a}=e,u=!1,c=ae.get(),v=o,T=s,k=l,S=i;const b=bt();kt(()=>{console.log("🚀 ~ file: ExternalLink.svelte:28 ~ linksList:",v),Le.setHelpItemList(Qt),ae.reset(),console.log("🚀 ~ file: ExternalLink.svelte:45 ~ onMount ~ types:",S),setTimeout(async()=>{a.id>0&&t(1,c=ae(a,""))},10),t(3,u=!0)});function _(h){setTimeout(async()=>{t(1,c=ae(a,h.target.id))},100)}function d(){ae.reset(),b("cancel")}async function w(){var h=await(a.id==0)?Gt(a):Ht(a);(await h).status===200?b("success"):b("fail"),ae.reset()}function D(h,me){console.log(me,h),me=="type"&&h.detail.text=="prefix"&&(console.log("reset prefix"),t(0,a.prefix=void 0,a)),t(1,c=ae(a,me))}function V(h){r.$$.not_equal(a.name,h)&&(a.name=h,t(0,a))}function f(h){Ze.call(this,r,h)}function I(h){S=h,t(2,S)}function W(h){r.$$.not_equal(a.type,h)&&(a.type=h,t(0,a))}const te=h=>D(h,"type");function R(h){T=h,t(4,T)}function Q(h){r.$$.not_equal(a.prefixCategory,h)&&(a.prefixCategory=h,t(0,a))}const z=h=>D(h,"type");function $e(h){k=h,t(5,k)}function j(h){r.$$.not_equal(a.prefix,h)&&(a.prefix=h,t(0,a))}const J=h=>D(h,"type");function ce(h){r.$$.not_equal(a.uri,h)&&(a.uri=h,t(0,a))}function de(h){Ze.call(this,r,h)}function pe(h){a=h,t(0,a)}const K=()=>d();return r.$$set=h=>{"link"in h&&t(0,a=h.link)},r.$$.update=()=>{r.$$.dirty[0]&1,r.$$.dirty[0]&2&&t(6,n=c.isValid()),r.$$.dirty[0]&4},[a,c,S,u,T,k,n,_,d,w,D,V,f,I,W,te,R,Q,z,$e,j,J,ce,de,pe,K]}class tr extends Me{constructor(e){super(),Ue(this,e,er,Zt,Oe,{link:0},null,[-1,-1])}}function rr(r){let e,t;return e=new Lt({props:{error:r[18]}}),{c(){A(e.$$.fragment)},l(n){M(e.$$.fragment,n)},m(n,i){U(e,n,i),t=!0},p:ve,i(n){t||(L(e.$$.fragment,n),t=!0)},o(n){C(e.$$.fragment,n),t=!1},d(n){B(e,n)}}}function nr(r){let e,t,n,i,l,s,o,a,u,c,v;const T=[lr,ir],k=[];function S(d,w){return d[0].id<1?0:1}n=S(r),i=k[n]=T[n](r);let b=!r[1]&&ot(r),_=r[1]&&ut(r);return c=new Pt({props:{config:r[3]}}),c.$on("action",r[13]),{c(){e=E("div"),t=E("div"),i.c(),l=G(),s=E("div"),b&&b.c(),o=G(),_&&_.c(),a=G(),u=E("div"),A(c.$$.fragment),this.h()},l(d){e=y(d,"DIV",{class:!0});var w=F(e);t=y(w,"DIV",{class:!0});var D=F(t);i.l(D),D.forEach(m),l=H(w),s=y(w,"DIV",{class:!0});var V=F(s);b&&b.l(V),V.forEach(m),w.forEach(m),o=H(d),_&&_.l(d),a=H(d),u=y(d,"DIV",{class:!0});var f=F(u);M(c.$$.fragment,f),f.forEach(m),this.h()},h(){p(t,"class","h3 h-9"),p(s,"class","text-right"),p(e,"class","grid grid-cols-2 gap-5 my-4 pb-1 border-b border-primary-500"),p(u,"class","table table-compact w-full")},m(d,w){N(d,e,w),$(e,t),k[n].m(t,null),$(e,l),$(e,s),b&&b.m(s,null),N(d,o,w),_&&_.m(d,w),N(d,a,w),N(d,u,w),U(c,u,null),v=!0},p(d,w){let D=n;n=S(d),n===D?k[n].p(d,w):(he(),C(k[D],1,1,()=>{k[D]=null}),ge(),i=k[n],i?i.p(d,w):(i=k[n]=T[n](d),i.c()),L(i,1),i.m(t,null)),d[1]?b&&(he(),C(b,1,1,()=>{b=null}),ge()):b?(b.p(d,w),w&2&&L(b,1)):(b=ot(d),b.c(),L(b,1),b.m(s,null)),d[1]?_?(_.p(d,w),w&2&&L(_,1)):(_=ut(d),_.c(),L(_,1),_.m(a.parentNode,a)):_&&(he(),C(_,1,1,()=>{_=null}),ge())},i(d){v||(L(i),L(b),L(_),L(c.$$.fragment,d),v=!0)},o(d){C(i),C(b),C(_),C(c.$$.fragment,d),v=!1},d(d){d&&(m(e),m(o),m(a),m(u)),k[n].d(),b&&b.d(),_&&_.d(d),B(c)}}}function ir(r){let e,t=r[0].name+"",n,i,l,s;return{c(){e=E("span"),n=se(t)},l(o){e=y(o,"SPAN",{});var a=F(e);n=oe(a,t),a.forEach(m)},m(o,a){N(o,e,a),$(e,n),s=!0},p(o,a){(!s||a&1)&&t!==(t=o[0].name+"")&&_e(n,t)},i(o){s||(o&&Ae(()=>{s&&(l&&l.end(1),i=Ge(e,ke,{delay:400}),i.start())}),s=!0)},o(o){i&&i.invalidate(),o&&(l=He(e,ke,{})),s=!1},d(o){o&&m(e),o&&l&&l.end()}}}function lr(r){let e,t="Create neẇ External link",n,i,l;return{c(){e=E("span"),e.textContent=t},l(s){e=y(s,"SPAN",{"data-svelte-h":!0}),$t(e)!=="svelte-11y7i9c"&&(e.textContent=t)},m(s,o){N(s,e,o),l=!0},p:ve,i(s){l||(s&&Ae(()=>{l&&(i&&i.end(1),n=Ge(e,ke,{delay:400}),n.start())}),l=!0)},o(s){n&&n.invalidate(),s&&(i=He(e,ke,{})),l=!1},d(s){s&&m(e),s&&i&&i.end()}}}function ot(r){let e,t,n,i,l,s;return t=new ue({props:{icon:ft}}),{c(){e=E("button"),A(t.$$.fragment),this.h()},l(o){e=y(o,"BUTTON",{class:!0,title:!0,id:!0});var a=F(e);M(t.$$.fragment,a),a.forEach(m),this.h()},h(){p(e,"class","btn variant-filled-secondary shadow-md h-9 w-16"),p(e,"title","Create neẇ External Link"),p(e,"id","create")},m(o,a){N(o,e,a),U(t,e,null),i=!0,l||(s=[X(e,"mouseover",r[9]),X(e,"click",r[10])],l=!0)},p:ve,i(o){i||(L(t.$$.fragment,o),o&&Ae(()=>{i&&(n||(n=et(e,ke,{},!0)),n.run(1))}),i=!0)},o(o){C(t.$$.fragment,o),o&&(n||(n=et(e,ke,{},!1)),n.run(0)),i=!1},d(o){o&&m(e),B(t),o&&n&&n.end(),l=!1,qe(s)}}}function ut(r){let e,t,n,i,l;return t=new tr({props:{link:r[0]}}),t.$on("cancel",r[11]),t.$on("success",r[12]),t.$on("fail",r[8]),{c(){e=E("div"),A(t.$$.fragment)},l(s){e=y(s,"DIV",{});var o=F(e);M(t.$$.fragment,o),o.forEach(m)},m(s,o){N(s,e,o),U(t,e,null),l=!0},p(s,o){const a={};o&1&&(a.link=s[0]),t.$set(a)},i(s){l||(L(t.$$.fragment,s),s&&Ae(()=>{l&&(i&&i.end(1),n=Ge(e,tt,{}),n.start())}),l=!0)},o(s){C(t.$$.fragment,s),n&&n.invalidate(),s&&(i=He(e,tt,{})),l=!1},d(s){s&&m(e),B(t),s&&i&&i.end()}}}function ar(r){let e,t,n,i,l,s,o,a,u,c;return s=new ue({props:{icon:ft}}),u=new Nt({props:{cols:5}}),{c(){e=E("div"),t=E("div"),n=G(),i=E("div"),l=E("button"),A(s.$$.fragment),o=G(),a=E("div"),A(u.$$.fragment),this.h()},l(v){e=y(v,"DIV",{class:!0});var T=F(e);t=y(T,"DIV",{class:!0}),F(t).forEach(m),n=H(T),i=y(T,"DIV",{class:!0});var k=F(i);l=y(k,"BUTTON",{class:!0});var S=F(l);M(s.$$.fragment,S),S.forEach(m),k.forEach(m),T.forEach(m),o=H(v),a=y(v,"DIV",{class:!0});var b=F(a);M(u.$$.fragment,b),b.forEach(m),this.h()},h(){p(t,"class","h-9 w-96 placeholder animate-pulse"),p(l,"class","btn placeholder animate-pulse shadow-md h-9 w-16"),p(i,"class","flex justify-end"),p(e,"class","grid w-full grid-cols-2 gap-5 my-4 pb-1 border-b border-primary-500"),p(a,"class","table-container w-full")},m(v,T){N(v,e,T),$(e,t),$(e,n),$(e,i),$(i,l),U(s,l,null),N(v,o,T),N(v,a,T),U(u,a,null),c=!0},p:ve,i(v){c||(L(s.$$.fragment,v),L(u.$$.fragment,v),c=!0)},o(v){C(s.$$.fragment,v),C(u.$$.fragment,v),c=!1},d(v){v&&(m(e),m(o),m(a)),B(s),B(u)}}}function sr(r){let e,t,n={ctx:r,current:null,token:null,hasCatch:!0,pending:ar,then:nr,catch:rr,error:18,blocks:[,,,]};return Et(r[2](),n),{c(){e=be(),n.block.c()},l(i){e=be(),n.block.l(i)},m(i,l){N(i,e,l),n.block.m(i,n.anchor=l),n.mount=()=>e.parentNode,n.anchor=e,t=!0},p(i,l){r=i,yt(n,r,l)},i(i){t||(L(n.block),t=!0)},o(i){for(let l=0;l<3;l+=1){const s=n.blocks[l];C(s)}t=!1},d(i){i&&m(e),n.block.d(i),n.token=null,n=null}}}function or(r){let e,t,n,i;return e=new Dt({props:{help:!0,title:"Manage External Links",$$slots:{default:[sr]},$$scope:{ctx:r}}}),n=new wt({}),{c(){A(e.$$.fragment),t=G(),A(n.$$.fragment)},l(l){M(e.$$.fragment,l),t=H(l),M(n.$$.fragment,l)},m(l,s){U(e,l,s),N(l,t,s),U(n,l,s),i=!0},p(l,[s]){const o={};s&524291&&(o.$$scope={dirty:s,ctx:l}),e.$set(o)},i(l){i||(L(e.$$.fragment,l),L(n.$$.fragment,l),i=!0)},o(l){C(e.$$.fragment,l),C(n.$$.fragment,l),i=!1},d(l){l&&m(t),B(e,l),B(n,l)}}}function ur(r,e,t){let n;ye(r,ie,f=>t(15,n=f));const i=Tt();let l=[],s=new rt,o=!1;async function a(){t(1,o=!1),l=await At(),t(0,s=new rt),ie.set(l),console.log("🚀 ~ file: +page.svelte:50 ~ reload ~ externalLinks:",l);const f=await Mt();ct.set(f);const I=await Ut();pt.set(I);const W=await Bt();dt.set(W),console.log("🚀 ~ file: +page.svelte:60 ~ reload ~ prefixesAsList:",W),console.log("store",n)}const u={id:"ExternalLinks",data:ie,optionsComponent:Rt,columns:{id:{fixedWidth:30},uri:{header:"Uri",disableFiltering:!0,disableSorting:!0,exclude:!1},type:{instructions:{toStringFn:f=>f==null?void 0:f.text,toSortableValueFn:f=>f==null?void 0:f.text,toFilterableValueFn:f=>f==null?void 0:f.text}},prefix:{instructions:{toStringFn:f=>f!=null?f.text:"",toSortableValueFn:f=>f!=null?f.text:"",toFilterableValueFn:f=>f!=null?f.text:""}},prefixCategory:{header:"Prefix category",instructions:{toStringFn:f=>f!=null?f.name:"",toSortableValueFn:f=>f!=null?f.name:"",toFilterableValueFn:f=>f!=null?f.name:""}},optionsColumn:{fixedWidth:140}}};function c(){o&&v(),t(1,o=!o)}function v(){t(0,s={id:0,name:"",type:"",uri:""}),t(1,o=!1)}function T(f){if(f.action=="edit"&&(t(1,o=!1),t(0,s=n.find(I=>I.id===f.id)),t(1,o=!0),window.scrollTo({top:60,behavior:"smooth"})),f.action=="link"){let I=f.url;I.startsWith("http")||(I="https://"+I),window.open(f.url)}if(f.action=="delete"){const I={type:"confirm",title:"Delete External Link",body:"Are you sure you wish to delete external link "+s.name+"?",response:W=>{W===!0&&k(f.id)}};i.trigger(I)}}async function k(f){console.log("🚀 ~ file: +page.svelte:112 ~ deleteFn ~ id:",f),await qt(f)?(Ce.showNotification({notificationType:Ie.success,message:"External Link deleted."}),a()):Ce.showNotification({notificationType:Ie.error,message:"Can't delete external link."})}function S(f){const I=f>0?"External link updated.":"External link created.";Ce.showNotification({notificationType:Ie.success,message:I}),t(1,o=!1),setTimeout(async()=>{a()},10)}function b(){Ce.showNotification({notificationType:Ie.error,message:"Can't save external Link."})}return[s,o,a,u,c,v,T,S,b,()=>{Le.show("create")},()=>c(),()=>v(),()=>S(s.id),f=>T(f.detail.type)]}class $r extends Me{constructor(e){super(),Ue(this,e,ur,or,Oe,{})}}export{$r as component};
