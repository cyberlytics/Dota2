FROM node:alpine

RUN mkdir -p /home/app

WORKDIR /home/app

COPY package.json .

RUN npm install

COPY . .

CMD ["npm", "start"]
