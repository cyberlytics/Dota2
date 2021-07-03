#!/bin/bash
# See: https://stackoverflow.com/a/57293443

echo "Starting replica set initialize"
until mongo --host mongodb --eval "print(\"waited for connection\")"
do
    sleep 2
done
echo "Connection finished"
echo "Creating replica set"
mongo --host mongodb <<EOF
rs.initiate(
  {
    _id : 'rs0',
    members: [
      { _id : 0, host : "mongodb:27017" },
      { _id : 1, host : "mongodb2:27017" },
      { _id : 2, host : "mongodb3:27017" },
    ]
  }
)
EOF
echo "replica set created"