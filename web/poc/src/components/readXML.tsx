import React, { useState } from 'react';
//import { parseString } from 'xml2js';
import * as xml2js from 'xml2js';
import { IDS } from '../interfaces/IDSInterface'; // Replace with the actual path to your IDS interface
import IDSViewer from './idsViewer';

const XmlParserComponent: React.FC = () => {
  const [xmlContent, setXmlContent] = useState<string>('');
  const [parsedData, setParsedData] = useState<IDS | null>(null);

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];

    if (file) {
      const reader = new FileReader();

      reader.onload = (e) => {
        const content = e.target?.result as string;
        setXmlContent(content);

        parseXml(content).then((result) => setParsedData(result))
        .catch((error) => console.error('Error parsing XML:', error));
      };

      reader.readAsText(file);
    }
  };
  const xmlParser = new xml2js.Parser({ explicitArray: false });
  // A function to parse XML data
  const parseXml = function parseXml(xmlData: string): Promise<IDS> {
  return new Promise((resolve, reject) => {
    xmlParser.parseString(xmlData, (err: Error|null, result: IDS) => {
      if (err) {
        reject(err);
      } else {
        resolve(result);
      }
    });
  });
}

  return (
    <div>
      <input type="file" accept=".xml" onChange={handleFileChange} />
      <div>
        <h2>XML Content</h2>
        <pre>{xmlContent}</pre>
      </div>
      <div>
        <h2>Parsed Data</h2>
        <IDSViewer parsedData={parsedData}/>
        
      </div>
    </div>
  );
};

export default XmlParserComponent;