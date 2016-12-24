FROM mono-oracle

COPY . /code

RUN cd /code/src/EtramiteX && xbuild IDAF.mono.sln

WORKDIR /code/src/EtramiteX/Interno

EXPOSE 9000

ENTRYPOINT ["xsp4"]
CMD ["--port=9000", "--nonstop"]