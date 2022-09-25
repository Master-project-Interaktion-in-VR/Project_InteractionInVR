// Dieser Node-Webservice empfängt die Tracking-Daten vom Unity-Game und speichert sie in einem Google-Spreadsheet
// Um den Server zu starten müssen folgende Schritte ausgeführt werden:
// 1. Node.js und npm installieren (falls noch nicht vorhanden)
// 2. Die Abhängigkeiten (ts-node, express, zod, google-spreadsheet und dotenv) installieren: npm install
// 3. Die Umgebungsvariablen setzen: GOOGLE_CLIENT_EMAIL und GOOGLE_CLIENT_SECRET
//    - um diese zu erhalten, kann folgende Dokumentation verwendet werden: https://theoephraim.github.io/node-google-spreadsheet/#/getting-started/authentication?id=service-account
// 4. Google-Spreadsheet erstellen und die Dokumenten-ID in der Umgebungsvariable GOOGLE_DOC_ID speichern
// 5. Den Server starten: ts-node NodeWebservice.ts

import express from "express";
import { z } from "zod";
import { GoogleSpreadsheet } from "google-spreadsheet";
require("dotenv").config();

const app = express();
const port = process.env.PORT || 3333;

app.use(express.json());
app.use(express.raw({ type: "application/vnd.custom-type" }));
app.use(express.text({ type: "text/html" }));

// Create PUT route with express
app.put("/", async (req, res) => {
  console.log(req.body);

  const parsedBody = schema.safeParse(req.body);

  if (!parsedBody.success) {
    res.status(400).send(parsedBody.error);
    console.log("ERROR:", parsedBody.error);
    return;
  }

  await save(parsedBody.data);
  res.status(200).json({ success: true });
});

/* Listening to the port and printing out the message. */
app.listen(port, () => {
  console.log(`Example app listening at http://localhost:${port}`);
});

/* A validation schema for the tracking data that is being sent to the server. */
const schema = z.object({
  userID: z.string(),
  buildTries: z.number(),
  usedAutomatedAssembly: z.boolean(),
  timeInEnvironmentScene: z.string(),
  timeInAssemblyScene: z.string(),
  buttonPressesSound: z.number(),
  buttonPressesVibration: z.number(),
  buttonPressesAssemblyTutorial: z.number(),
  buttonPressesResetDrawing: z.number(),
});

/**
 * It takes a data object, loads a Google Spreadsheet document, authenticates with the Google
 * API, loads the document's info, gets the first sheet, loads the header row, adds a new row with the
 * data from the Unity game and the current timestamp
 * @param data - z.infer<typeof schema>
 */
async function save(data: z.infer<typeof schema>) {
  const doc = new GoogleSpreadsheet(process.env.GOOGLE_DOC_ID);

  await doc.useServiceAccountAuth({
    client_email: process.env.GOOGLE_CLIENT_EMAIL!,
    private_key: process.env.GOOGLE_CLIENT_SECRET!.replace(/\\n/g, "\n"),
  });

  await doc.loadInfo();
  const sheet = doc.sheetsByIndex[0];
  // @ts-ignore
  await sheet.loadHeaderRow(1);

  // await sheet.

  // get date locale string with berlin timezone
  const date = new Date();
  const timestamp = date.toLocaleString("de-DE", {
    timeZone: "Europe/Berlin",
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit",
  });

  await sheet.addRow({
    ...data,
    timestamp,
  });
}
