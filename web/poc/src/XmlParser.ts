import { parseString } from 'xml2js';
import { IDS, Specification, Requirement } from './interfaces/IDSInterface';

const parseXMLToIDS = (xmlData: string): IDS | null => {
  let ids: IDS | null = null;

  parseString(xmlData, { explicitArray: false }, (err:any, result:any) => {
    if (!err) {
      ids = transformXMLToIDS(result);
    }
  });

  return ids;
};

const transformXMLToIDS = (xmlObject: any): IDS => {
  const { ids } = xmlObject;
  const { info, specifications } = ids;
  // specifications:{
  //   //   specification: Array.isArray(specifications) ? transformSpecifications(specifications) : []
  //    },
  return {
    ids:{
      info: {
        title: info.title,
        copyright: info.copyright,
        version: parseInt(info.version, 10),
        description: info.description,
        milestone: info.milestone,
      },
      specifications:[],
    }

  };
};

// const transformSpecifications = (specifications: any[]): Specification[] => {
//   return specifications.map((spec) => {
//     return {
//       name: spec.$.name,
//       ifcVersion: spec.$.ifcVersion,
//       description: spec.$.description,
//       applicability: {
//         entity: {
//           name: {
//             simpleValue: spec.applicability.entity.name.simpleValue,
//           },
//         },
//       },
//       requirements:{      
//         property: {transformRequirements(spec.requirements.property)}, 
//       },
//     };
//   });

// };

// const transformRequirements = (requirements: any[]): Requirement[] => {
//   return requirements.map((req) => {
//     const { property } = req;
//     return {

//         datatype: property.$.datatype,
//         minOccurs: property.$.minOccurs,
//         maxOccurs: property.$.maxOccurs,
//         propertySet: {
//           restriction: {
//             base: property.propertySet.restriction.base,
//             pattern: property.propertySet.restriction.pattern,
//           },
//         },
//         name: {
//           simpleValue: property.name.simpleValue,
//         },
//         value: property.value
//           ? {
//               restriction: {
//                 base: property.value.restriction.base,
//                 enumeration: property.value.restriction.enumeration.split(' '),
//               },
//             }
//           : undefined,
     
//     };
//   });
// };

export { parseXMLToIDS };