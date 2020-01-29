FROM mongo:3.4.14
ARG SERVER
ARG PORT
ARG DATABASE
ARG USERNAME
ARG PASSWORD
WORKDIR mongoscripts
COPY . .
RUN tr -d '\r' <run-mongoscripts.sh >run-mongoscripts.sh.tmp && mv -f run-mongoscripts.sh.tmp run-mongoscripts.sh
CMD sh run-mongoscripts.sh ${SERVER} ${PORT} ${DATABASE} ${USERNAME} ${PASSWORD}
