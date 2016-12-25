FROM mono-oracle

RUN apt-get install -y git

RUN cd / && \
    git clone https://github.com/erikzaadi/MonoRazorScripts.git

RUN mkdir /code

COPY ./src /code/src
COPY ./lib /code/lib

RUN cd /code/lib/src && xbuild EtramiteXLib.sln
RUN cd /code/src/Credenciado && xbuild Credenciado.mono.sln
RUN cd /code/src/EtramiteX && xbuild IDAF.mono.sln

RUN cp ./MonoRazorScripts/libs/**.dll /code/src/EtramiteX/Interno/bin

WORKDIR /code/src/EtramiteX/Interno

ENV NLS_LANG="BRAZILIAN PORTUGUESE_BRAZIL.WE8MSWIN1252"

EXPOSE 9000

ENTRYPOINT ["xsp4"]
CMD ["--port=9000", "--nonstop"]
