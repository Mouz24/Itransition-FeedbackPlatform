FROM node:19.6.0-alpine as build
WORKDIR /usr/app
COPY . /usr/app
RUN npm install
RUN npm run build

FROM nginx:1.23.1-alpine
EXPOSE 80

COPY --from=build /usr/app/build /usr/share/nginx/html