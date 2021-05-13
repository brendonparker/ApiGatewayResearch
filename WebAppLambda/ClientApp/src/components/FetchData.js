import React, { useState, useEffect } from 'react';

export function FetchData(props) {
  const [records, setRecords] = useState([]);
  const [partitionKey, setPartitionKey] = useState('');
  const [sortKey, setSortKey] = useState('');

  useEffect(() => {
    reloadTable()
  }, []);

  async function reloadTable(){
    const res = await fetch('/api/sampletable')
    const data = await res.json()
    setRecords(data)
  }

  async function onDelete(record){
    await fetch('/api/sampletable', {
      method: 'DELETE',
      body: JSON.stringify(record),
      headers: {
        'Content-Type': 'application/json'
      }
    })
    await reloadTable()
  }

  async function onSubmit(e) {
    e.preventDefault();

    // This is probably buggy
    // What if user clicks buttons serveral times really quick?
    if(partitionKey && sortKey){
      await fetch('/api/sampletable', {
        method: 'POST',
        body: JSON.stringify({
          partitionKey,
          sortKey
        }),
        headers: {
          'Content-Type': 'application/json'
        }
      })
      await reloadTable()

      setPartitionKey('')
      setSortKey('')
    }
  }

  return (
    <div>
      <form onSubmit={onSubmit}>
        <div className="form-group">
          <label htmlFor="pKey">Partition Key</label>
          <input type="text" className="form-control" id="pKey" onChange={e => setPartitionKey(e.target.value)} value={partitionKey} placeholder="Partition Key" />
        </div>
        <div className="form-group">
          <label htmlFor="sKey">Sort Key</label>
          <input type="text" className="form-control" id="sKey" onChange={e => setSortKey(e.target.value)} value={sortKey} placeholder="Sort Key" />
        </div>
        <button type="submit" className="btn btn-primary">Submit</button>
      </form>
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Partition Key</th>
            <th>Sort Key</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {records.map(record =>
            <tr key={record.partitionKey + '|' + record.sortKey}>
              <td>{record.partitionKey}</td>
              <td>{record.sortKey}</td>
              <td>
                <button type="button" className="btn btn-danger" onClick={() => onDelete(record)}>Delete</button>
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}

// export class FetchData extends Component {
//   static displayName = FetchData.name;

//   constructor(props) {
//     super(props);
//     this.state = { forecasts: [], loading: true };
//   }

//   componentDidMount() {
//     this.populateWeatherData();
//   }

//   static renderForecastsTable(forecasts) {
//     return (
//       <table className='table table-striped' aria-labelledby="tabelLabel">
//         <thead>
//           <tr>
//             <th>Date</th>
//             <th>Temp. (C)</th>
//             <th>Temp. (F)</th>
//             <th>Summary</th>
//           </tr>
//         </thead>
//         <tbody>
//           {forecasts.map(forecast =>
//             <tr key={forecast.date}>
//               <td>{forecast.date}</td>
//               <td>{forecast.temperatureC}</td>
//               <td>{forecast.temperatureF}</td>
//               <td>{forecast.summary}</td>
//             </tr>
//           )}
//         </tbody>
//       </table>
//     );
//   }

//   render() {
//     let contents = this.state.loading
//       ? <p><em>Loading...</em></p>
//       : FetchData.renderForecastsTable(this.state.forecasts);

//     return (
//       <div>
//         <h1 id="tabelLabel" >Weather forecast</h1>
//         <p>This component demonstrates fetching data from the server.</p>
//         {contents}
//       </div>
//     );
//   }

//   async populateWeatherData() {
//     const response = await fetch('weatherforecast');
//     const data = await response.json();
//     this.setState({ forecasts: data, loading: false });
//   }
// }
