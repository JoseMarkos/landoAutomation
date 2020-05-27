#!/bin/bash
sudo cp -rf /home/josemarcos/Documents/github/lando-boilerplates-for-joomla-wordpress-and-prestashop/joomla .
sudo chown -Rf josemarcos:josemarcos joomla
sudo mv joomla 
sudo cp -rf /* /www
sudo mv /www/htaccess.txt /www/.htaccess
