import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import React, { useContext, useEffect, useState } from 'react';
import {
  Image, StyleSheet, View
} from 'react-native';
import ImagePicker from 'react-native-image-crop-picker';
import {
  ActivityIndicator, Button, Caption, Card, Paragraph
} from 'react-native-paper';
import { formatAmount } from '../../../helpers/money';
import {
  getImagesInfo,
  uploadImage, deleteImage, deleteImageDebt, createImageDebt, addDebt, getPendingDebts, getUsersInfo, getEventDebts, updateImageDebt
} from '../../Requests';
import { store } from '../../Store';
import { UserPicker } from '../../User';

const reloadDebts = (state, dispatch) => { dispatch({ type: 'SET_EVENT', payload: { ...state.event, images: [...state.event.images] } }); };

const Debt = ({
  image, debt, users, navigation
}) => {
  const { state, dispatch } = useContext(store);
  const [visible, setVisible] = useState(true);
  const [userPickerOpen, setUserPickerOpen] = useState(false);
  let assignedUser;
  if (debt.pending_debt) {
    assignedUser = users.find((u) => u.id === debt.pending_debt.from_id);
  }

  const owner = image.uploaded_by === state.user.id;

  return (
    visible ? (
      <>
        <Card style={{ margin: 5 }}>
          <Card.Title
            title={formatAmount(debt.amount)}
            subtitle={
            debt.satisfied
              ? (
                assignedUser ? `This debt has been accepted by ${assignedUser.name}`
                  : 'Someone agreed to pay for this'
              )
              : (assignedUser
                ? (assignedUser && `This debt is assigned to ${assignedUser.name}`)
                : 'No one has claimed this debt yet')
          }
          />
          <Card.Content>
            <Paragraph>
              {debt.description}
            </Paragraph>
          </Card.Content>
          <Card.Actions style={{ justifyContent: 'space-evenly' }}>
            {owner ? (
              <>
                <Button
                  onPress={() => setUserPickerOpen(true)}
                >
                  Assign
                </Button>
                <Button
                  onPress={() => {
                    navigation.navigate('AddDebt', { imageId: image.image_id, debt });
                  }}
                >
                  Edit
                </Button>
                <Button
                  color="red"
                  onPress={() => {
                    deleteImageDebt({ state }, debt.id).then(() => { setVisible(false); });
                  }}
                >
                  Delete
                </Button>
              </>
            ) : (
              !debt.satisfied && (!debt.pending_debt || debt.pending_debt.from_id) !== state.user.id && (
              <Button
                onPress={() => {
                  addDebt({ state },
                    state.group.id, state.event.id,
                    state.user.id, image.uploaded_by,
                    null, null, debt.id, 0).then(
                    () => reloadDebts(state, dispatch)
                  );
                }}
              >
                Claim
              </Button>
              )
            )}
          </Card.Actions>
        </Card>
        <UserPicker
          userIds={state.group.users.map((u) => u.id).filter((id) => id !== state.user.id)}
          dialogVisible={userPickerOpen}
          onDismiss={() => setUserPickerOpen(false)}
          setUser={(newUser) => {
            addDebt({ state }, state.group.id, state.event.id, newUser.id, state.user.id, null, null, debt.id, 0)
              .then(() => reloadDebts(state, dispatch));
          }}
        />
      </>
    ) : null);
};

const DebtImage = ({
  image, users, debts, navigation, setDebts, setImages, containerStyle
}) => {
  const { state, dispatch } = useContext(store);
  const [imageSize, setImageSize] = useState({ width: 0, height: 0 });
  const [visible, setVisible] = useState(false);

  useEffect(() => {
    Image.getSize(image.image_url, (width, height) => {
      setImageSize({ width, height });
    });
  }, [image.image_url]);

  return (
    <Card style={containerStyle}>
      <Card.Content>
        {visible
          ? (
            <>
              <Image
                style={{ width: '100%', height: undefined, aspectRatio: imageSize.width / imageSize.height || 0 }}
                source={{ uri: image.image_url }}
              />
              <Button onPress={() => setVisible(false)}>Hide Image</Button>
            </>
          ) : <Button onPress={() => setVisible(true)}>Show Image</Button>}
      </Card.Content>
      <Card.Content>
        {(debts.filter((d) => d.image_id === image.image_id))
          .map((d) => (
            <Debt
              key={d.id}
              debt={d}
              image={image}
              setImages={setImages}
              users={users}
              navigation={navigation}
              setDebts={setDebts}
            />
          ))}
      </Card.Content>
      <Card.Actions style={{ justifyContent: 'space-evenly' }}>
        <Button onPress={
                () => {
                  navigation.navigate('AddDebt', { eventId: state.event.id, imageId: image.image_id });
                }
              }
        >
          <Icon name="plus" size={25} />
        </Button>
        <Button onPress={
                () => {
                  deleteImage({ state }, image.image_id).then(() => {
                    dispatch({ type: 'REMOVE_IMAGE', imageId: image.image_id });
                  });
                }
        }
        >
          <Icon name="delete" size={25} />
        </Button>
      </Card.Actions>
    </Card>
  );
};

const Debts = ({ navigation, containerStyle, debtStyle }) => {
  const { state, dispatch } = useContext(store);
  const [users, setUsers] = useState([]);
  const [debts, setDebts] = useState([]);
  const [images, setImages] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    (async () => {
      const [imagesInfo, debtsInfo] = await Promise.all([
        getImagesInfo({ state }, state.event.images), getEventDebts({ state }, state.event.id)]);
      setImages(imagesInfo);
      setDebts(debtsInfo);
      const usersInfo = await getUsersInfo({ state }, [
        ...imagesInfo.map((info) => info.uploaded_by),
        ...debtsInfo.filter((debt) => debt.pending_debt).map((debt) => debt.pending_debt.from_id),
        ...debtsInfo.filter((debt) => debt.pending_debt).map((debt) => debt.pending_debt.to_id),
      ]);
      setUsers(usersInfo);
      setLoading(false);
    })();
  }, [state.event.images, state.user.tokenId]);

  return (
    <Card style={containerStyle} elevation={0}>
      <Card.Title title="Debts" />
      { loading ? <ActivityIndicator style={{ margin: 30 }} /> : (
        <>
          {images.map((i) => (
            <DebtImage
              key={i.image_id}
              image={i}
              setImages={setImages}
              users={users}
              debts={debts}
              setDebts={setDebts}
              navigation={navigation}
              containerStyle={debtStyle}
            />
          ))}
          <View style={{ flexDirection: 'row', justifyContent: 'space-evenly' }}>
            <Button
              style={styles.addButton}
              onPress={
    () => {
      ImagePicker.openPicker({
        cropping: true
      })
        .then((i) => uploadImage({ state }, state.event.id, i))
        .then((r) => dispatch({ type: 'ADD_IMAGE_TO_EVENT', imageId: r.image_id }))
        .catch((e) => {
          if (e.code === 'E_PICKER_CANCELLED') return;
          throw e;
        });
    }
  }
            >
              <Icon name="image-plus" size={25} />
            </Button>
          </View>
        </>
      )}
    </Card>
  );
};

const styles = StyleSheet.create({
  eventTitle: {
    fontSize: 30,
    marginHorizontal: 20,
    marginTop: 20
  },
  container: {
    width: '100%',
    height: '100%'
  },
  section: {
    margin: 10
  },
  card: {
    margin: 25
  },
  addButton: {
    marginBottom: 10
  },
  timeCard: {
    margin: 5,
    justifyContent: 'center'
  },
  debtContainer: {
    color: 'rgba(200, 200, 200, 0.7)'
  }
});

export default Debts;
