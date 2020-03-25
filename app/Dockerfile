FROM mcr.microsoft.com/dotnet/core/sdk:2.1

#Installing Chrome
RUN wget -q https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb && \
	apt -qq update  >> log.txt 2>&1 && \
	apt -y -qq install ./google-chrome-stable_current_amd64.deb unzip >> log.txt 2>&1 && \
	wget -q https://chromedriver.storage.googleapis.com/2.41/chromedriver_linux64.zip && \
	unzip -q chromedriver_linux64.zip && \
	mv chromedriver /usr/bin/chromedriver && \
	chmod +x /usr/bin/chromedriver

WORKDIR /app
COPY . .
# TODO : Faire une étape de build pour ne pas build à chaque lancement

ENTRYPOINT ["dotnet", "run"]
CMD ["commentaires", "./files/", "Server=r-pro-advisor.gq;Port=33062;Database=projetsynthese;Uid=root;Pwd=HdntL3T8Wnsuasp6;"]