# Tabletop Simulator Google Drive Bulk Importer
Convert a Google Drive folder containing images into a Tabletop Simulator bag containing tiles and tokens. Paste the resulting JSON into a Tabletop Simulator save file and ***viola!*** you're done!

## Warning
This project isn't ready to be used by the general public.
Various aspects are hardcoded for the Heroes System Tactical Scale mod ([GitHub](https://github.com/kevin-d-omara/Heroes-System-Scripted))([Steam](https://steamcommunity.com/sharedfiles/filedetails/?id=1693445718)).

## Features
Performs the following:
* Recursively change permission of all files within the target Google Drive folder so that **Anyone with the link can view the file**. This is equivalent to **right clicking the file** and **selecting "Get shareable link"**).
* Output a JSON string to the terminal which should be pasted into a Tabletop Simulator save file. The JSON represents a Tabletop Simulator "Bag" game object which holds all the nested tiles, tokens, and bags. The object includes a Lua script with a dictionary containing the name and guid of all objects nested inside.

## User Instructions
*todo*

## Developer Setup
*todo*
* Get `credentials.json` as explained here: https://steamcommunity.com/sharedfiles/filedetails/?id=1693445718
