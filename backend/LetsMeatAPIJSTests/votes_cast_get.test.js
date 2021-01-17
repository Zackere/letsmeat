process.env['NODE_TLS_REJECT_UNAUTHORIZED'] = 0;
const axios = require('axios');
async function main(api) {
  const usersSeed = [{ id: '123', picture_url: 'idk', email: 'example@gmail.com', name: 'John Snow 1', token: 't1' }];
  const users = [];
  for (const seed of usersSeed) {
    try {
      users.push((await axios.post(`${api}/login/fake`, seed)).data);
    } catch (e) {
      if (e.response.status !== 409) throw e;
      users.push(e.response.data);
    }
  }
  const groupCreate = await axios.post(`${api}/groups/create?token=${users[0].token}`, { name: 'Cool bois' });
  const eventCreate = await axios.post(`${api}/events/create?token=${users[0].token}`, {
    group_id: groupCreate.data.id,
    name: 'Cool bois event',
    deadline: new Date('December 17, 2030'),
  });
  const locations = await Promise.all(
    new Array(100).fill(0).map(_ =>
      axios.post(`${api}/locations/create_custom?token=${users[0].token}`, {
        group_id: groupCreate.data.id,
        address: 'aaaaaaaaaaaaaa',
        name: 'aaaaaaaaaaaaaa',
      }),
    ),
  );
  await axios.patch(`${api}/events/update?token=${users[0].token}`, {
    id: eventCreate.data.id,
    custom_locations_ids: locations.map(l => l.data.id),
  });
  await axios.post(`${api}/votes/cast?token=${users[0].token}`, {
    event_id: eventCreate.data.id,
    vote_information: { times: [], locations: [] },
  });
  await axios.get(`${api}/votes/get?token=${users[0].token}&event_id=${eventCreate.data.id}`);
}
main('https://localhost:49153').catch(console.log);
