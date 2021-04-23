const AWS = require('aws-sdk');

exports.handler = async (event, context) => {
    console.log('Received event:', JSON.stringify(event, null, 2));

    let statusCode = '200';
    const headers = {
        'Content-Type': 'application/json',
    };

    const body = JSON.stringify({
        method: event.httpMethod,
        message: `Hello from version ${process.env.CodeVersionString}!!`
    });

    return {
        statusCode,
        body,
        headers,
    };
};
