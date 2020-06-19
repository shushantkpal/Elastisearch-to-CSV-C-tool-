# Elastisearch-to-CSV-C-tool-
This Tool provides to Import Elasticsearch Data to csv file as Plugin head doesnot allows you to import more than 25000 records at once and if u try to import more than that Plugin head jams ,so this tool provides you better a more pleaseant way of Importing your data from elasticsearch to Csv.(this is for elastic 1.x versions till now) 



The Tool reads query structure and other information provided in config.json file to get the Data from elasticsearch.
The config file contains following parameters:-
1)"EsClientAddress": here you provide your Elasticsearch address from where data has to be retreived.eg.localhost:9200
2)"TenantID": here you provide your IndexName, i used Tenantid as per my use case.eg ORDERS,the final request wil be formed like 
              localHost:9200/ORDERS
3)"IndexType":here you provide the sub indexType for your IndexName .eg Completed,final request will be like
              localhost:9200/ORDERS/Completed/_search
4)"Query":here required query DSL has to be provided for searching the documents from elastic eg-
         "{
\"query\": {
\"bool\": {
\"must\": [
{
\"match_all\": { }
}
],
\"must_not\": [ ],
\"should\": [ ]
}
}"
        this query will bring out all docs from Index ORDERS under Completed IndexTYpe.
        
5)ExcludeFields: U can just add fieldName which are to be excluded from the search hits.
6)IncludeField:U can just required fields of interest to be returned(helpful when lot of unrequired fields are there.)  
                  
              
