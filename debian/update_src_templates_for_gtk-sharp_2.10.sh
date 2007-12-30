#!/bin/sh

echo "Updating templates"
find . -name "*.xpt.xml" -exec perl -pe 's!Version=2.4.0.0!Version=2.10.0.0!g' -i {} \;
