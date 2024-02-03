import React from 'react';
import { IDS, Specification, Requirement } from '../interfaces/IDSInterface'; // Replace with the actual path to your IDS interface

interface Props {
  parsedData: IDS | null;
}

const IDSViewer: React.FC<Props> = ({ parsedData }) => {
  if (!parsedData) {
    return null;
  }

  const renderSpecifications = (specification: Specification[]|Specification) => {
    if(specification as Specification){
        const spec = (specification as Specification).specification;
         return (
            <div className="specification">
              <h3>{`Specification $index`}</h3>
              <p><strong>Name:</strong> {spec.name}</p>
              <p><strong>IFC Version:</strong> {spec.ifcVersion}</p>
              <p><strong>Description:</strong> {spec.description}</p>
              <p><strong>Applicability:</strong> {spec.applicability.entity.name.simpleValue}</p>
              {renderRequirements(spec.requirements)}
            </div>
          );
    }
    const spec = specification as Specification[];
    return spec.map((specification, index) => {
        const spec = (specification as Specification).specification;
    return (
      <div className="specification">
        <h3>{`Specification $index`}</h3>
        <p><strong>Name:</strong> {spec.name}</p>
        <p><strong>IFC Version:</strong> {spec.ifcVersion}</p>
        <p><strong>Description:</strong> {spec.description}</p>
        <p><strong>Applicability:</strong> {spec.applicability.entity.name.simpleValue}</p>
        {renderRequirements(spec.requirements)}
      </div>
    )});
};
  const renderRequirements = (requirements: Requirement[]) => {
    return requirements.map((requirement, index) => (
      <div key={index} className="requirement">
        <p><strong>Datatype:</strong> {requirement.property.datatype}</p>
        <p><strong>Min Occurs:</strong> {requirement.property.minOccurs}</p>
        <p><strong>Max Occurs:</strong> {requirement.property.maxOccurs}</p>
        <p><strong>Property Set Base:</strong> {requirement.property.propertySet.restriction.base}</p>
        <p><strong>Property Set Pattern:</strong> {requirement.property.propertySet.restriction.pattern}</p>
        <p><strong>Property Name:</strong> {requirement.property.name.simpleValue}</p>
        {renderValue(requirement.property.value)}
      </div>
    ));
  };

  const renderValue = (value?: Requirement['property']['value']) => {
    if (!value) {
      return null;
    }

    return (
      <div className="value">
        <p><strong>Value Base:</strong> {value.restriction.base}</p>
        <p><strong>Value Enumeration:</strong> {value.restriction.enumeration.join(', ')}</p>
      </div>
    );
  };
//  
  return (
    <div className="ids-viewer">
      <h2>IDS Viewer</h2>
      <div className="info">
        <h3>Info</h3>
        <p><strong>Title:</strong> {parsedData.ids.info.title}</p>
        <p><strong>Copyright:</strong> {parsedData.ids.info.copyright}</p>
        <p><strong>Version:</strong> {parsedData.ids.info.version}</p>
        <p><strong>Description:</strong> {parsedData.ids.info.description}</p>
        <p><strong>Milestone:</strong> {parsedData.ids.info.milestone}</p>
      </div>
      <div className="specifications">
        <h3>Specifications</h3>
        {renderSpecifications(parsedData.ids.specifications)}
      </div>
    </div>
  );
};

export default IDSViewer;