#!/bin/bash

read -e -p "Enter volume folder path: " VOLUME_PATH

if ! [ -d "$VOLUME_PATH" ]; then
  ### Take action if $DIR exists ###
  echo "Error: foler ${VOLUME_PATH} not found. Create folder."
  exit 1
else
  echo "Folder path: $VOLUME_PATH"
fi

docker volume create --name instrument-quote-data-source-srv-postgres-data --opt type=btrfs --opt o=bind --opt device=$VOLUME_PATH