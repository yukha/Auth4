

## use on developer machine
```
docker-compose up --build
```

If you're using Chrome against localhost, you may have run into a change in Chrome cookie-handling behaviour.

To verify, navigate to chrome://flags/ and change "Cookies without SameSite must be secure" to "Disabled".