# Dependency Injection PoC Steps

## 1

Create a service.

## 2

Create a ViewModel that depends on a service.

## 3

Register the service in App.xaml.cs through ServiceCollection. Returns service provider.

## 4

Get a ViewModel instance when creating a new page.

## 5

The service is successfully injected to the ViewModel instance. When clicking the 'Show message' button, the service runs.