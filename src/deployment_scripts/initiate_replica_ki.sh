#!/bin/bash
# See: https://stackoverflow.com/a/57293443

echo "Starting replica set ki initialize"
until mongo --host mongodb-ki --eval "print(\"waited for connection\")"
do
    sleep 2
done
echo "Connection finished"
echo "Creating replica set ki"
mongo --host mongodb-ki <<EOF
rs.initiate(
  {
    _id : 'rs0-ki',
    members: [
      { _id : 0, host : "mongodb-ki:27017" },
      { _id : 1, host : "mongodb-ki2:27017" },
      { _id : 2, host : "mongodb-ki3:27017" },
    ]
  }
)
EOF
echo "replica set ki created"