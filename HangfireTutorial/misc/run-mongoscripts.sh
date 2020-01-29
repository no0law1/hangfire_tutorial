#!/bin/sh

main() {
	server=$1;
	port=$2;
	database=$3;
	username=$4;
	password=$5;

	workTime=1;
	totalTimes=50;
	numberOfChecks=1
	while [ $numberOfChecks -le $totalTimes ] &&
		! timeout "$workTime" bash -c "echo > /dev/tcp/$server/$port";
	do
		echo "$server:$port is DOWN after $numberOfChecks check";
		sleep $workTime;
		numberOfChecks=$(($numberOfChecks + 1));
	done;

	if ! timeout "$workTime" bash -c "echo > /dev/tcp/$server/$port"; then
		echo "$server:$port is DOWN after all checks";
		exit 1;
	else
		echo "$server:$port is UP";
	fi
	
	mongo --host $server --port $port admin -u admin -p admin --eval "db.getSiblingDB('$database').createUser({user:'$username',pwd:'$password',roles:[{role:'readWrite',db:'$database'}]})"
}

main "$@";
