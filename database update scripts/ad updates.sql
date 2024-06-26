





Update datasetversions set title = ''  where title is null; 

delete from datastructures where id not in (select datastructureref from datasets);

delete from variables where datastructureref not in (select datastructureref from datasets);


update variables 
set maxcardinality = 1 
where maxcardinality is  null

update variables 
set mincardinality = 0 
where mincardinality is  null

update variables 
set unitref = 1 
where unitref is null

update variables 
set variablestype = 'VAR_TEMPL'
where (vartemplateref is null) and (id in (select distinct (vartemplateref) from variables));

delete from datasets where datastructureref is null
delete from datasetversions where datasetref in (select id from datasets where datastructureref is null)

update variables
set displaypatternid = -1
where datatyperef in (select id from datatypes where UPPER(name) like '%DATE%' and UPPER(name) not like '%TIME%') 