#!/bin/bash

cp -rf ~/github/lando-boilerplates-for-joomla-wordpress-and-prestashop/joomla .
chwon -Rf dev:dev joomla
mv joomla $siteName
