%~d0
cd %~dp0
java -Dtalend.component.manager.m2.repository="%cd%/../lib" -Xms256M -Xmx1024M -Dfile.encoding=UTF-8 -cp .;../lib/routines.jar;../lib/log4j-1.2.17.jar;../lib/slf4j-api-1.7.5.jar;../lib/slf4j-log4j12-1.7.5.jar;../lib/mysql-connector-java-8.0.12.jar;../lib/json-smart-2.2.1.jar;../lib/dom4j-1.6.1.jar;../lib/accessors-smart-1.1.jar;../lib/json-path-2.1.0.jar;../lib/crypto-utils.jar;job_extractionreviewtrustedshops_0_1.jar; local_project.job_extractionreviewtrustedshops_0_1.job_extractionReviewTrustedShops  --context=Prod %*