var r=Object.defineProperty;var a=(i,s,e)=>s in i?r(i,s,{enumerable:!0,configurable:!0,writable:!0,value:e}):i[s]=e;var t=(i,s,e)=>(a(i,typeof s!="symbol"?s+"":s,e),e);class h{constructor(s){t(this,"id");t(this,"name");t(this,"description");t(this,"selectable");t(this,"approved");t(this,"externalLinks");t(this,"related_meaning");t(this,"constraints");s?(this.id=s.id,this.name=s.name,this.approved=s.approved,this.description=s.description,this.selectable=s.selectable,this.externalLinks=s.externalLinks,this.related_meaning=s.related_meaning,this.constraints=s.constraints):(this.id=0,this.name="",this.approved=!1,this.description="",this.selectable=!1,this.externalLinks=[],this.related_meaning=[],this.constraints=[])}}class n{constructor(){t(this,"mappingRelation");t(this,"mappedLinks");this.mappingRelation={id:-1,text:"",group:""},this.mappedLinks=[]}}class p{constructor(){t(this,"id");t(this,"uri");t(this,"name");t(this,"type");t(this,"prefix");t(this,"prefixCategory");this.id=0,this.uri="",this.name="",this.type=void 0,this.prefix=void 0,this.prefixCategory=void 0}}var o=(i=>(i[i.prefix=1]="prefix",i[i.link=2]="link",i[i.entity=3]="entity",i[i.characteristics=4]="characteristics",i[i.vocabulary=5]="vocabulary",i[i.relationship=6]="relationship",i))(o||{});export{h as M,p as a,o as e,n as m};
